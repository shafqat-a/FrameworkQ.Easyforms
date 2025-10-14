/**
 * Copied from frontend/src/js/formruntime.js for UI hosting
 */
(function(window, $) {
    'use strict';
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
        _init: function() {
            this._state.formId = this.$form.attr('data-form');
            this._state.formVersion = this.$form.attr('data-version') || '1.0';
            this._state.formTitle = this.$form.attr('data-title') || '';
            this._bindChangeEvents();
            this._initTables();
            this._initExpressions();
            this._initValidation();
            this._initComposites();
            this._initConditionalVisibility();
            this._initDataFetch();
            this._emit('ready', this);
        },
        _bindChangeEvents: function() {
            var self = this;
            this.$form.on('change', 'input, select, textarea', function() {
                var $f = $(this), name = $f.attr('name');
                var oldValue = self._state[name];
                var newValue = $f.val();
                self._state[name] = newValue;
                self._emit('change', { fieldName: name, oldValue: oldValue, newValue: newValue, element: this });
            });
        },
        getValue: function(includeComputed) {
            includeComputed = includeComputed !== false;
            var data = { form_id: this._state.formId, form_version: this._state.formVersion };
            this.$form.find('input[name], select[name], textarea[name]').each(function() {
                var $field = $(this); var name = $field.attr('name'); var type = $field.attr('type');
                var isComputed = $field.attr('data-compute') || $field.attr('data-computed');
                if (!includeComputed && isComputed) return;
                if (type === 'checkbox') { data[name] = $field.is(':checked'); }
                else if (type === 'radio') { if ($field.is(':checked')) { data[name] = $field.val(); } }
                else { data[name] = $field.val(); }
            });
            if (window.CompositeWidgets){ var cstate = window.CompositeWidgets.getState(this.$form); if (Object.keys(cstate).length>0){ data._composites = cstate; } }
            return data;
        },
        setValue: function(data, triggerChange) {
            triggerChange = triggerChange !== false; var self = this;
            $.each(data, function(name, value){ var $field = self.$form.find('[name="' + name + '"]'); if ($field.length===0) return;
                var type = $field.attr('type');
                if (type === 'checkbox') $field.prop('checked', !!value);
                else if (type === 'radio') $field.filter('[value="' + value + '"]').prop('checked', true);
                else $field.val(value);
                if (triggerChange) $field.trigger('change');
            });
            if (data._composites && window.CompositeWidgets){ window.CompositeWidgets.setState(self.$form, data._composites); }
        },
        submit: function(options) {
            options = options || {}; var self = this;
            var data = { formId: this._state.formId, formVersion: this._state.formVersion, status: options.status || 'submitted', data: this.getValue(true) };
            var submitEvent = $.Event('formdsl:submit'); this.$form.trigger(submitEvent, [data]);
            if (submitEvent.isDefaultPrevented()) { return $.Deferred().reject({ message: 'Submission cancelled' }).promise(); }
            return $.ajax({ url: (this.options.apiBaseUrl + '/v1/submissions'), method: 'POST', contentType: 'application/json', data: JSON.stringify(data) })
                .done(function(result){ self._emit('submit:success', result); })
                .fail(function(xhr){ var err = xhr.responseJSON || { message: 'Submission failed' }; self._emit('submit:error', err); });
        },
        saveDraft: function() { return this.submit({ status: 'draft' }); },
        reset: function(clearData) { if (clearData) { this.$form[0].reset(); this._state = { formId: this._state.formId, formVersion: this._state.formVersion, formTitle: this._state.formTitle }; } else { this.$form[0].reset(); } },
        _initTables: function() { if (window.TableManager) { window.TableManager.initializeTables(this.$form); } this.$form.find('table[data-table]').formTable(); },
        _initExpressions: function(){ var self = this; this.$form.find('[data-compute]').each(function(){ $(this).attr('readonly', true).addClass('computed-field'); });
            this.$form.on('change', 'input, select, textarea', function(){ self._recalculateComputedFields(); }); this._recalculateComputedFields(); },
        _recalculateComputedFields: function(){ var self = this; this.$form.find('[data-compute]').each(function(){ var $field = $(this), formula = $field.attr('data-compute'); if (!formula) return; var context = self.getValue(false);
            if (window.ExpressionEvaluator) { try { var result = window.ExpressionEvaluator.evaluate(formula, context); $field.val(result ?? ''); } catch(e) { console.error('Eval failed', formula, e);} } }); },
        _initValidation: function(){ if (window.ValidationModule) { window.ValidationModule.initialize(this.$form, { validateOn: this.options.validateOn, showInlineErrors: this.options.showInlineErrors, showErrorSummary: this.options.showErrorSummary, errorSummarySelector: this.options.errorSummarySelector }); } },
        _initComposites: function(){ if (window.CompositeWidgets){ window.CompositeWidgets.initialize(this.$form); } },
        _initConditionalVisibility: function(){ var self = this; this.$form.find('[data-when]').each(function(){ var $el = $(this); $el.data('whenCondition', $el.attr('data-when')); });
            this.$form.on('change', 'input, select, textarea', function(){ self._evaluateConditionalVisibility(); }); this._evaluateConditionalVisibility(); },
        _evaluateConditionalVisibility: function(){ var self = this; this.$form.find('[data-when]').each(function(){ var $el = $(this); var condition = $el.data('whenCondition'); if (!condition || !window.ExpressionEvaluator) return; try { var ctx = self.getValue(false); var res = window.ExpressionEvaluator.evaluate(condition, ctx); if (res) { $el.removeClass('hidden').show(); } else { $el.addClass('hidden').hide(); } } catch(e){ console.error('Condition failed', condition, e);} }); },
        validate: function(fieldName){ if (!window.ValidationModule) return { valid:true, errors:{} }; var r = window.ValidationModule.validate(this.$form, fieldName); var c = window.ValidationModule.validateConstraints(this.$form); var combined = { valid: r.valid && c.valid, errors: $.extend({}, r.errors, c.errors) }; this._emit('validation', combined); return combined; },
        _initDataFetch: function(){ if (window.DataFetchModule) { window.DataFetchModule.initialize(this.$form, { apiBaseUrl: this.options.apiBaseUrl, fetchTimeout: this.options.fetchTimeout, fetchRetries: this.options.fetchRetries }); } },
        _emit: function(eventName, data){ this.$form.trigger('formdsl:' + eventName, [data]); },
        _log: function(message, data){ if (this.options.debug) { console.log('[FormRuntime]', message, data || ''); } },
        destroy: function(){ this.$form.off('.formdsl'); this._state = {}; this._listeners = []; }
    };
    window.FormRuntimeHTMLDSL = { mount: function(formElement, options){ var $form = $(formElement); if ($form.length===0) throw new Error('Form element not found'); if ($form.length>1) throw new Error('Multiple forms found'); var runtime = new FormRuntime($form, options); $form.data('formRuntime', runtime); return runtime; }, getInstance: function(formElement){ var $form = $(formElement); return $form.data('formRuntime') || null; } };
})(window, jQuery);
