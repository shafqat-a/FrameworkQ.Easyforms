/**
 * FrameworkQ.Easyforms - Validation Module
 * Client-side validation with native HTML5 and data-* attribute support
 */

(function(window, $) {
    'use strict';

    window.ValidationModule = {
        /**
         * Initialize validation for a form
         * @param {jQuery} $form - Form element
         * @param {Object} options - Validation options
         */
        initialize: function($form, options) {
            var settings = $.extend({
                validateOn: 'change',
                showInlineErrors: true,
                showErrorSummary: true,
                errorSummarySelector: '[data-error-summary]'
            }, options);

            // Mark all required fields
            this._markRequiredFields($form);

            // Bind validation events based on validateOn setting
            this._bindValidationEvents($form, settings);

            // Initialize error summary
            if (settings.showErrorSummary) {
                this._initErrorSummary($form, settings.errorSummarySelector);
            }

            console.log('[Validation] Initialized');
        },

        /**
         * Validate entire form or specific field
         * @param {jQuery} $form - Form element
         * @param {string} fieldName - Optional field name (validates all if omitted)
         * @returns {Object} Validation result
         */
        validate: function($form, fieldName) {
            var errors = {};
            var $fields;

            if (fieldName) {
                $fields = $form.find('[name="' + fieldName + '"]');
            } else {
                $fields = $form.find('input, select, textarea');
            }

            var self = this;
            $fields.each(function() {
                var $field = $(this);
                var name = $field.attr('name');
                if (!name) return;

                var fieldErrors = self._validateField($field, $form);
                if (fieldErrors.length > 0) {
                    errors[name] = fieldErrors;
                }
            });

            return {
                valid: Object.keys(errors).length === 0,
                errors: errors
            };
        },

        /**
         * Validate a single field
         * @private
         */
        _validateField: function($field, $form) {
            var errors = [];
            var value = $field.val();
            var name = $field.attr('name');

            // Native HTML5 validation
            if ($field[0].checkValidity && !$field[0].checkValidity()) {
                errors.push($field[0].validationMessage);
            }

            // data-required
            if ($field.attr('data-required') === 'true' || $field.attr('required') !== undefined) {
                if (!value || value.trim() === '') {
                    var msg = $field.attr('data-error-required') || 'This field is required';
                    errors.push(msg);
                }
            }

            // data-required-when (conditional required)
            var requiredWhen = $field.attr('data-required-when');
            if (requiredWhen && window.ExpressionEvaluator) {
                try {
                    var context = this._buildContext($form);
                    var isRequired = window.ExpressionEvaluator.evaluate(requiredWhen, context);
                    if (isRequired && (!value || value.trim() === '')) {
                        var msg = $field.attr('data-error-required') || 'This field is required';
                        errors.push(msg);
                    }
                } catch (e) {
                    console.error('[Validation] Failed to evaluate required-when:', requiredWhen, e);
                }
            }

            // data-pattern
            var pattern = $field.attr('data-pattern') || $field.attr('pattern');
            if (pattern && value) {
                var regex = new RegExp(pattern);
                if (!regex.test(value)) {
                    var msg = $field.attr('data-error-pattern') || 'Value does not match required pattern';
                    errors.push(msg);
                }
            }

            // data-min / data-max (for numbers)
            if ($field.attr('type') === 'number' || $field.attr('data-type') === 'integer' || $field.attr('data-type') === 'decimal') {
                var numValue = parseFloat(value);
                if (!isNaN(numValue)) {
                    var min = $field.attr('data-min') || $field.attr('min');
                    var max = $field.attr('data-max') || $field.attr('max');

                    if (min && numValue < parseFloat(min)) {
                        var msg = $field.attr('data-error-min') || 'Value must be at least ' + min;
                        errors.push(msg);
                    }

                    if (max && numValue > parseFloat(max)) {
                        var msg = $field.attr('data-error-max') || 'Value must be at most ' + max;
                        errors.push(msg);
                    }
                }
            }

            // Update field UI
            if (errors.length > 0) {
                this._showFieldErrors($field, errors);
            } else {
                this._clearFieldErrors($field);
            }

            return errors;
        },

        /**
         * Validate cross-field constraints
         * @param {jQuery} $form - Form element
         * @returns {Object} Constraint validation results
         */
        validateConstraints: function($form) {
            var errors = {};
            var self = this;

            // Validate constraints on containers
            $form.find('[data-constraint]').each(function() {
                var $container = $(this);
                var constraint = $container.attr('data-constraint');
                var message = $container.attr('data-constraint-message') || 'Constraint violation';

                if (!window.ExpressionEvaluator) {
                    return;
                }

                try {
                    var context = self._buildContext($form, $container);
                    var result = window.ExpressionEvaluator.evaluate(constraint, context);

                    if (!result) {
                        var containerId = $container.attr('id') || 'constraint';
                        errors[containerId] = [message];
                        self._showConstraintError($container, message);
                    } else {
                        self._clearConstraintError($container);
                    }
                } catch (e) {
                    console.error('[Validation] Failed to evaluate constraint:', constraint, e);
                }
            });

            return {
                valid: Object.keys(errors).length === 0,
                errors: errors
            };
        },

        /**
         * Show field errors
         * @private
         */
        _showFieldErrors: function($field, errors) {
            $field.addClass('is-invalid');

            // Remove existing error message
            $field.next('.invalid-feedback').remove();

            // Add new error message
            var $error = $('<div class="invalid-feedback">' + errors.join(', ') + '</div>');
            $field.after($error);
        },

        /**
         * Clear field errors
         * @private
         */
        _clearFieldErrors: function($field) {
            $field.removeClass('is-invalid');
            $field.next('.invalid-feedback').remove();
        },

        /**
         * Show constraint error
         * @private
         */
        _showConstraintError: function($container, message) {
            $container.addClass('constraint-error');
            var $existing = $container.find('> .constraint-feedback').first();
            if ($existing.length === 0) {
                $container.prepend('<div class="constraint-feedback invalid-feedback">' + message + '</div>');
            }
        },

        /**
         * Clear constraint error
         * @private
         */
        _clearConstraintError: function($container) {
            $container.removeClass('constraint-error');
            $container.find('> .constraint-feedback').remove();
        },

        /**
         * Build context for expression evaluation
         * @private
         */
        _buildContext: function($form, $scope) {
            var context = {};
            var $fields = $scope ? $scope.find('input, select, textarea') : $form.find('input, select, textarea');

            $fields.each(function() {
                var $field = $(this);
                var name = $field.attr('name') || $field.attr('data-col-name');
                if (name) {
                    // Remove row index suffix
                    name = name.replace(/_\d+$/, '');
                    var val = $field.val();
                    context[name] = val === '' ? null : (isNaN(val) ? val : parseFloat(val));
                }
            });

            return context;
        },

        /**
         * Mark required fields
         * @private
         */
        _markRequiredFields: function($form) {
            $form.find('[required], [data-required="true"]').each(function() {
                var $field = $(this);
                if (!$field.hasClass('required-marked')) {
                    $field.addClass('required-marked');
                }
            });
        },

        /**
         * Bind validation events
         * @private
         */
        _bindValidationEvents: function($form, settings) {
            var self = this;
            var validateOn = settings.validateOn;

            $form.on(validateOn, 'input, select, textarea', function() {
                var $field = $(this);
                self._validateField($field, $form);
            });

            // Always validate on submit
            $form.on('submit', function(e) {
                var result = self.validate($form);
                var constraintResult = self.validateConstraints($form);

                if (!result.valid || !constraintResult.valid) {
                    e.preventDefault();
                    self._updateErrorSummary($form, $.extend({}, result.errors, constraintResult.errors));
                    self._scrollToFirstError($form);
                    return false;
                }
            });
        },

        /**
         * Initialize error summary panel
         * @private
         */
        _initErrorSummary: function($form, selector) {
            var $summary = $(selector);
            if ($summary.length === 0) {
                $summary = $('<div data-error-summary style="display:none;"></div>');
                $form.before($summary);
            }
            $summary.html('<h4>Please correct the following errors:</h4><ul id="error-list"></ul>');
        },

        /**
         * Update error summary panel
         * @private
         */
        _updateErrorSummary: function($form, errors) {
            var $summary = $('[data-error-summary]');
            var $list = $summary.find('#error-list');
            $list.empty();

            if (Object.keys(errors).length > 0) {
                $.each(errors, function(field, messages) {
                    $.each(messages, function(i, msg) {
                        $list.append('<li>' + field + ': ' + msg + '</li>');
                    });
                });
                $summary.show();
            } else {
                $summary.hide();
            }
        },

        /**
         * Scroll to first error
         * @private
         */
        _scrollToFirstError: function($form) {
            var $firstError = $form.find('.is-invalid, .constraint-error').first();
            if ($firstError.length > 0) {
                $firstError[0].scrollIntoView({ behavior: 'smooth', block: 'center' });
                $firstError.focus();
            }
        }
    };

})(window, jQuery);
