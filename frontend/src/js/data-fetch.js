/**
 * FrameworkQ.Easyforms - Data Fetch Module
 * External API data fetching with caching and cascading support
 */

(function(window, $) {
    'use strict';

    window.DataFetchModule = {
        _cache: {},

        /**
         * Initialize data fetch for all fields with data-fetch
         * @param {jQuery} $form - Form element
         * @param {Object} options - Fetch options
         */
        initialize: function($form, options) {
            var settings = $.extend({
                apiBaseUrl: '/api',
                fetchTimeout: 5000,
                fetchRetries: 2
            }, options);

            var self = this;

            // Find all fields with data-fetch
            $form.find('[data-fetch]').each(function() {
                var $field = $(this);
                var config = self._parseFetchConfig($field);

                // Store config on element
                $field.data('fetchConfig', config);

                // Bind fetch trigger events
                self._bindFetchEvents($field, $form, config, settings);

                // Auto-fetch on load if configured
                if (config.fetchOn.indexOf('load') !== -1) {
                    self.fetchOptions($field, $form, settings);
                }
            });

            // Bind cascading dependencies
            this._bindCascadeDependencies($form, settings);

            console.log('[DataFetch] Initialized');
        },

        /**
         * Fetch options for a field
         * @param {jQuery} $field - Field element
         * @param {jQuery} $form - Form element
         * @param {Object} settings - Settings
         * @returns {Promise} jQuery promise
         */
        fetchOptions: function($field, $form, settings) {
            var config = $field.data('fetchConfig');
            if (!config) {
                return $.Deferred().reject('No fetch configuration').promise();
            }

            // Build fetch URL with token substitution
            var url = this._substituteTokens(config.url, $form, $field);

            // Check cache
            var cacheKey = this._getCacheKey(url, config.cache);
            if (cacheKey && this._cache[cacheKey]) {
                this._populateOptions($field, this._cache[cacheKey], config.map);
                return $.Deferred().resolve(this._cache[cacheKey]).promise();
            }

            // Fetch from API via proxy
            var proxyUrl = settings.apiBaseUrl + '/v1/proxy/fetch';
            var params = {
                endpoint: url,
                method: config.method
            };

            var self = this;

            return $.ajax({
                url: proxyUrl,
                method: 'GET',
                data: params,
                timeout: settings.fetchTimeout
            }).done(function(data) {
                // Cache response
                if (cacheKey) {
                    self._cache[cacheKey] = data;
                }

                // Populate options
                self._populateOptions($field, data, config.map);
            }).fail(function(xhr) {
                console.error('[DataFetch] Fetch failed:', xhr);
            });
        },

        /**
         * Parse fetch configuration from field attributes
         * @private
         */
        _parseFetchConfig: function($field) {
            var dataFetch = $field.attr('data-fetch') || '';
            var parts = dataFetch.split(':', 2);

            return {
                method: parts.length > 1 ? parts[0] : 'GET',
                url: parts.length > 1 ? parts[1] : parts[0],
                fetchOn: ($field.attr('data-fetch-on') || 'focus').split(','),
                minChars: parseInt($field.attr('data-min-chars')) || 0,
                debounceMs: parseInt($field.attr('data-fetch-debounce')?.replace('ms', '')) || 300,
                map: $field.attr('data-fetch-map'),
                cache: $field.attr('data-fetch-cache') || 'session',
                depends: $field.attr('data-depends')?.split(',').map(function(s) { return s.trim(); }) || []
            };
        },

        /**
         * Bind fetch events (focus, input, change)
         * @private
         */
        _bindFetchEvents: function($field, $form, config, settings) {
            var self = this;

            config.fetchOn.forEach(function(event) {
                event = event.trim();

                if (event === 'focus') {
                    $field.on('focus', function() {
                        self.fetchOptions($field, $form, settings);
                    });
                } else if (event === 'input' || event === 'change') {
                    // Debounce for input/change events
                    var debounceTimer;
                    $field.on(event, function() {
                        clearTimeout(debounceTimer);
                        debounceTimer = setTimeout(function() {
                            var value = $field.val();
                            if (value && value.length >= config.minChars) {
                                self.fetchOptions($field, $form, settings);
                            }
                        }, config.debounceMs);
                    });
                }
            });
        },

        /**
         * Bind cascade dependencies
         * @private
         */
        _bindCascadeDependencies: function($form, settings) {
            var self = this;

            $form.find('[data-depends]').each(function() {
                var $field = $(this);
                var depends = $field.attr('data-depends').split(',');

                depends.forEach(function(selector) {
                    selector = selector.trim();
                    var $parent = $(selector);

                    $parent.on('change', function() {
                        // Clear current field
                        $field.val('');

                        // Reload options
                        self.fetchOptions($field, $form, settings);
                    });
                });
            });
        },

        /**
         * Substitute tokens in URL with field values
         * @private
         */
        _substituteTokens: function(url, $form, $field) {
            var result = url;

            // Find all {token} patterns
            var tokens = url.match(/\{([^}]+)\}/g) || [];

            tokens.forEach(function(token) {
                var fieldName = token.slice(1, -1); // Remove { }

                var value = '';
                if (fieldName === 'search' || fieldName === 'value') {
                    value = $field.val() || '';
                } else {
                    var $sourceField = $form.find('[name="' + fieldName + '"]');
                    value = $sourceField.val() || '';
                }

                result = result.replace(token, encodeURIComponent(value));
            });

            return result;
        },

        /**
         * Populate field options from fetched data
         * @private
         */
        _populateOptions: function($field, data, mapConfig) {
            if (!Array.isArray(data)) {
                console.warn('[DataFetch] Expected array, got:', typeof data);
                return;
            }

            // Parse map config (e.g., "value:id,label:name")
            var map = { value: 'value', label: 'label' };
            if (mapConfig) {
                mapConfig.split(',').forEach(function(part) {
                    var kv = part.split(':');
                    if (kv.length === 2) {
                        map[kv[0].trim()] = kv[1].trim();
                    }
                });
            }

            // Populate select options
            if ($field.is('select')) {
                var currentValue = $field.val();
                $field.empty();
                $field.append('<option value="">-- Select --</option>');

                data.forEach(function(item) {
                    var value = item[map.value] || item.value || item.id;
                    var label = item[map.label] || item.label || item.name || value;
                    $field.append('<option value="' + value + '">' + label + '</option>');
                });

                if (currentValue) {
                    $field.val(currentValue);
                }
            }
            // TODO: Support autocomplete for input fields
        },

        /**
         * Get cache key
         * @private
         */
        _getCacheKey: function(url, cacheMode) {
            if (cacheMode === 'none') {
                return null;
            }

            if (cacheMode === 'session' || cacheMode.startsWith('ttl:')) {
                return 'fetch:' + url;
            }

            return null;
        }
    };

})(window, jQuery);
