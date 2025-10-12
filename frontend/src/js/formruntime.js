/**
 * FrameworkQ.Easyforms - jQuery Form Runtime
 * Main runtime initialization and state management
 */

(function(window, $) {
    'use strict';

    /**
     * FormRuntime class
     * @param {jQuery} $form - Form element
     * @param {Object} options - Configuration options
     */
    function FormRuntime($form, options) {
        this.$form = $form;
        this.options = $.extend({}, FormRuntime.DEFAULTS, options);
        this._state = {};
        this._listeners = [];
        this._init();
    }

    FormRuntime.DEFAULTS = {
        apiBaseUrl: '/api',
        autoSave: false,
        autoSaveInterval: 30000,
        validateOn: 'change',
        showInlineErrors: true,
        showErrorSummary: true,
        errorSummarySelector: '[data-error-summary]',
        fetchTimeout: 5000,
        fetchRetries: 2,
        debug: false
    };

    FormRuntime.prototype = {
        /**
         * Initialize runtime
         * @private
         */
        _init: function() {
            this._log('Initializing form runtime');

            // Initialize state from form metadata
            this._state.formId = this.$form.attr('data-form');
            this._state.formVersion = this.$form.attr('data-version') || '1.0';
            this._state.formTitle = this.$form.attr('data-title') || '';

            // Bind change event listeners
            this._bindChangeEvents();

            // Initialize tables (US2)
            this._initTables();

            // Initialize expressions (US2)
            this._initExpressions();

            // Initialize validation (US3)
            this._initValidation();

            // Initialize conditional visibility (US3)
            this._initConditionalVisibility();

            // Initialize data fetching (US6)
            this._initDataFetch();

            // Emit ready event
            this._emit('ready', this);
            this._log('Form runtime ready');
        },

        /**
         * Bind change events to form fields
         * @private
         */
        _bindChangeEvents: function() {
            var self = this;

            this.$form.on('change', 'input, select, textarea', function(e) {
                var $field = $(this);
                var fieldName = $field.attr('name');
                var oldValue = self._state[fieldName];
                var newValue = $field.val();

                self._state[fieldName] = newValue;

                self._emit('change', {
                    fieldName: fieldName,
                    oldValue: oldValue,
                    newValue: newValue,
                    element: this
                });
            });
        },

        /**
         * Get current form values as JSON
         * @param {boolean} includeComputed - Include computed field values
         * @returns {Object} Form data
         */
        getValue: function(includeComputed) {
            includeComputed = includeComputed !== false; // Default to true

            var data = {
                form_id: this._state.formId,
                form_version: this._state.formVersion
            };

            // Serialize all input fields
            this.$form.find('input[name], select[name], textarea[name]').each(function() {
                var $field = $(this);
                var name = $field.attr('name');
                var type = $field.attr('type');
                var isComputed = $field.attr('data-compute') || $field.attr('data-computed');

                // Skip computed fields if not requested
                if (!includeComputed && isComputed) {
                    return;
                }

                if (type === 'checkbox') {
                    data[name] = $field.is(':checked');
                } else if (type === 'radio') {
                    if ($field.is(':checked')) {
                        data[name] = $field.val();
                    }
                } else {
                    data[name] = $field.val();
                }
            });

            this._log('getValue() called', data);
            return data;
        },

        /**
         * Set form values from JSON
         * @param {Object} data - Data object
         * @param {boolean} triggerChange - Fire change events
         */
        setValue: function(data, triggerChange) {
            triggerChange = triggerChange !== false; // Default to true
            this._log('setValue() called', data);

            var self = this;

            $.each(data, function(name, value) {
                var $field = self.$form.find('[name="' + name + '"]');

                if ($field.length === 0) {
                    return;
                }

                var type = $field.attr('type');

                if (type === 'checkbox') {
                    $field.prop('checked', !!value);
                } else if (type === 'radio') {
                    $field.filter('[value="' + value + '"]').prop('checked', true);
                } else {
                    $field.val(value);
                }

                if (triggerChange) {
                    $field.trigger('change');
                }
            });
        },

        /**
         * Submit form
         * @param {Object} options - Submission options
         * @returns {Promise} Submission result
         */
        submit: function(options) {
            options = options || {};
            var self = this;

            this._log('submit() called');

            // Get form data
            var data = {
                formId: this._state.formId,
                formVersion: this._state.formVersion,
                status: options.status || 'submitted',
                data: this.getValue(true)
            };

            // Emit submit event (can be cancelled)
            var submitEvent = $.Event('formdsl:submit');
            this.$form.trigger(submitEvent, [data]);

            if (submitEvent.isDefaultPrevented()) {
                this._log('Submission cancelled by event handler');
                return $.Deferred().reject({ message: 'Submission cancelled' }).promise();
            }

            // Submit to API
            return $.ajax({
                url: options.url || (this.options.apiBaseUrl + '/v1/submissions'),
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data)
            }).done(function(result) {
                self._emit('submit:success', result);
                self._log('Submission successful', result);
            }).fail(function(xhr) {
                var error = xhr.responseJSON || { message: 'Submission failed' };
                self._emit('submit:error', error);
                self._log('Submission failed', error);
            });
        },

        /**
         * Save draft
         * @returns {Promise} Save result
         */
        saveDraft: function() {
            this._log('saveDraft() called');
            return this.submit({ status: 'draft' });
        },

        /**
         * Reset form
         * @param {boolean} clearData - Clear all data
         */
        reset: function(clearData) {
            this._log('reset() called');

            if (clearData) {
                this.$form[0].reset();
                this._state = {
                    formId: this._state.formId,
                    formVersion: this._state.formVersion,
                    formTitle: this._state.formTitle
                };
            } else {
                // Reset to initial values
                this.$form[0].reset();
            }
        },

        /**
         * Initialize tables (US2)
         * @private
         */
        _initTables: function() {
            // Initialize all tables using TableManager
            if (window.TableManager) {
                window.TableManager.initializeTables(this.$form);
            }

            // Initialize table widgets using jQuery plugin
            this.$form.find('table[data-table]').formTable();
        },

        /**
         * Initialize expressions (US2)
         * @private
         */
        _initExpressions: function() {
            var self = this;

            // Mark all computed fields as readonly
            this.$form.find('[data-compute]').each(function() {
                $(this).attr('readonly', true).addClass('computed-field');
            });

            // Bind change events to recalculate computed fields
            this.$form.on('change', 'input, select, textarea', function() {
                self._recalculateComputedFields();
            });

            // Initial calculation
            this._recalculateComputedFields();
        },

        /**
         * Recalculate all computed fields
         * @private
         */
        _recalculateComputedFields: function() {
            var self = this;

            this.$form.find('[data-compute]').each(function() {
                var $field = $(this);
                var formula = $field.attr('data-compute');

                if (!formula) {
                    return;
                }

                // Build context from current form values
                var context = self.getValue(false);

                // Evaluate formula
                if (window.ExpressionEvaluator) {
                    try {
                        var result = window.ExpressionEvaluator.evaluate(formula, context);
                        $field.val(result !== null && result !== undefined ? result : '');
                    } catch (e) {
                        console.error('[FormRuntime] Failed to evaluate formula:', formula, e);
                    }
                }
            });
        },

        /**
         * Initialize validation (US3)
         * @private
         */
        _initValidation: function() {
            if (window.ValidationModule) {
                window.ValidationModule.initialize(this.$form, {
                    validateOn: this.options.validateOn,
                    showInlineErrors: this.options.showInlineErrors,
                    showErrorSummary: this.options.showErrorSummary,
                    errorSummarySelector: this.options.errorSummarySelector
                });
            }
        },

        /**
         * Initialize conditional visibility (US3)
         * @private
         */
        _initConditionalVisibility: function() {
            var self = this;

            // Find all elements with data-when
            this.$form.find('[data-when]').each(function() {
                var $element = $(this);
                $element.data('whenCondition', $element.attr('data-when'));
            });

            // Bind change events to evaluate conditions
            this.$form.on('change', 'input, select, textarea', function() {
                self._evaluateConditionalVisibility();
            });

            // Initial evaluation
            this._evaluateConditionalVisibility();
        },

        /**
         * Evaluate all conditional visibility rules
         * @private
         */
        _evaluateConditionalVisibility: function() {
            var self = this;

            this.$form.find('[data-when]').each(function() {
                var $element = $(this);
                var condition = $element.data('whenCondition');

                if (!condition || !window.ExpressionEvaluator) {
                    return;
                }

                try {
                    var context = self.getValue(false);
                    var result = window.ExpressionEvaluator.evaluate(condition, context);

                    if (result) {
                        $element.removeClass('hidden').show();
                    } else {
                        $element.addClass('hidden').hide();
                    }
                } catch (e) {
                    console.error('[FormRuntime] Failed to evaluate condition:', condition, e);
                }
            });
        },

        /**
         * Validate form (US3)
         * @param {string} fieldName - Optional field name
         * @returns {Object} Validation result
         */
        validate: function(fieldName) {
            if (!window.ValidationModule) {
                return { valid: true, errors: {} };
            }

            var result = window.ValidationModule.validate(this.$form, fieldName);
            var constraintResult = window.ValidationModule.validateConstraints(this.$form);

            var combinedResult = {
                valid: result.valid && constraintResult.valid,
                errors: $.extend({}, result.errors, constraintResult.errors)
            };

            this._emit('validation', combinedResult);
            return combinedResult;
        },

        /**
         * Initialize data fetching (US6)
         * @private
         */
        _initDataFetch: function() {
            if (window.DataFetchModule) {
                window.DataFetchModule.initialize(this.$form, {
                    apiBaseUrl: this.options.apiBaseUrl,
                    fetchTimeout: this.options.fetchTimeout,
                    fetchRetries: this.options.fetchRetries
                });
            }
        },

        /**
         * Emit custom event
         * @private
         */
        _emit: function(eventName, data) {
            this.$form.trigger('formdsl:' + eventName, [data]);
        },

        /**
         * Log message if debug enabled
         * @private
         */
        _log: function(message, data) {
            if (this.options.debug) {
                console.log('[FormRuntime]', message, data || '');
            }
        },

        /**
         * Destroy runtime
         */
        destroy: function() {
            this._log('Destroying runtime');
            this.$form.off('.formdsl');
            this._state = {};
            this._listeners = [];
        }
    };

    /**
     * FormRuntimeHTMLDSL global API
     */
    window.FormRuntimeHTMLDSL = {
        /**
         * Mount form runtime on a form element
         * @param {HTMLFormElement|jQuery|string} formElement - Form element
         * @param {Object} options - Configuration options
         * @returns {FormRuntime} Runtime instance
         */
        mount: function(formElement, options) {
            var $form = $(formElement);

            if ($form.length === 0) {
                throw new Error('Form element not found');
            }

            if ($form.length > 1) {
                throw new Error('Multiple forms found - please specify a unique selector');
            }

            var runtime = new FormRuntime($form, options);

            // Store runtime instance on form element
            $form.data('formRuntime', runtime);

            return runtime;
        },

        /**
         * Get runtime instance from form element
         * @param {HTMLFormElement|jQuery|string} formElement - Form element
         * @returns {FormRuntime|null} Runtime instance
         */
        getInstance: function(formElement) {
            var $form = $(formElement);
            return $form.data('formRuntime') || null;
        }
    };

})(window, jQuery);
