/**
 * Copied from frontend/src/js/validation.js for UI hosting
 */
(function(window, $) {
    'use strict';
    window.ValidationModule = {
        initialize: function($form, options){
            var settings = $.extend({ validateOn:'change', showInlineErrors:true, showErrorSummary:true, errorSummarySelector:'[data-error-summary]' }, options);
            this._markRequiredFields($form); this._bindValidationEvents($form, settings); if (settings.showErrorSummary) this._initErrorSummary($form, settings.errorSummarySelector);
        },
        validate: function($form, fieldName){ var errors = {}; var $fields = fieldName ? $form.find('[name="'+fieldName+'"]') : $form.find('input, select, textarea'); var self = this; $fields.each(function(){ var $f=$(this); var name=$f.attr('name'); if(!name) return; var es=self._validateField($f, $form); if (es.length>0) errors[name]=es; }); return { valid: Object.keys(errors).length===0, errors: errors } },
        _validateField: function($field, $form){ var errors=[]; var value=$field.val(); if ($field[0].checkValidity && !$field[0].checkValidity()) errors.push($field[0].validationMessage);
            if ($field.attr('data-required')==='true' || $field.attr('required')!==undefined){ if (!value || value.trim()===''){ errors.push($field.attr('data-error-required')||'This field is required'); } }
            var requiredWhen=$field.attr('data-required-when'); if (requiredWhen && window.ExpressionEvaluator){ try{ var ctx = this._buildContext($form); var isReq = window.ExpressionEvaluator.evaluate(requiredWhen, ctx); if (isReq && (!value || value.trim()==='')) errors.push($field.attr('data-error-required')||'This field is required'); } catch(e){ console.error('required-when', e);} }
            var pattern=$field.attr('data-pattern')||$field.attr('pattern'); if (pattern && value){ var re=new RegExp(pattern); if (!re.test(value)) errors.push($field.attr('data-error-pattern')||'Value does not match required pattern'); }
            if ($field.attr('type')==='number' || $field.attr('data-type')==='integer' || $field.attr('data-type')==='decimal'){
                var n=parseFloat(value); if (!isNaN(n)){ var min=$field.attr('data-min')||$field.attr('min'); var max=$field.attr('data-max')||$field.attr('max'); if (min && n<parseFloat(min)) errors.push($field.attr('data-error-min')||('Value must be at least '+min)); if (max && n>parseFloat(max)) errors.push($field.attr('data-error-max')||('Value must be at most '+max)); }
            }
            if (errors.length>0) this._showFieldErrors($field, errors); else this._clearFieldErrors($field); return errors; },
        validateConstraints: function($form){ var errors={}; var self=this; $form.find('[data-constraint]').each(function(){ var $c=$(this), c=$c.attr('data-constraint'), msg=$c.attr('data-constraint-message')||'Constraint violation'; if (!window.ExpressionEvaluator) return; try{ var ctx=self._buildContext($form, $c); var result=window.ExpressionEvaluator.evaluate(c, ctx); if(!result){ var id=$c.attr('id')||'constraint'; errors[id]=[msg]; self._showConstraintError($c, msg);} else { self._clearConstraintError($c);} } catch(e){ console.error('constraint', e);} }); return { valid:Object.keys(errors).length===0, errors:errors } },
        _showFieldErrors: function($field, errors){ $field.addClass('is-invalid'); $field.next('.invalid-feedback').remove(); $field.after($('<div class="invalid-feedback">'+errors.join(', ')+'</div>')); },
        _clearFieldErrors: function($field){ $field.removeClass('is-invalid'); $field.next('.invalid-feedback').remove(); },
        _showConstraintError: function($c, msg){ $c.addClass('constraint-error'); var $e=$c.find('> .constraint-feedback').first(); if ($e.length===0) $c.prepend('<div class="constraint-feedback invalid-feedback">'+msg+'</div>'); },
        _clearConstraintError: function($c){ $c.removeClass('constraint-error'); $c.find('> .constraint-feedback').remove(); },
        _buildContext: function($form, $scope){ var ctx={}; var $fields = $scope ? $scope.find('input, select, textarea') : $form.find('input, select, textarea'); $fields.each(function(){ var $f=$(this); var name=$f.attr('name')||$f.attr('data-col-name'); if (name){ name=name.replace(/_\d+$/, ''); var val=$f.val(); ctx[name]= val==='' ? null : (isNaN(val) ? val : parseFloat(val)); } }); return ctx; },
        _markRequiredFields: function($form){ $form.find('[required], [data-required="true"]').each(function(){ var $f=$(this); if(!$f.hasClass('required-marked')) $f.addClass('required-marked'); }); },
        _bindValidationEvents: function($form, settings){ var self=this; $form.on(settings.validateOn, 'input, select, textarea', function(){ self._validateField($(this), $form); }); $form.on('submit', function(e){ var r=self.validate($form); var c=self.validateConstraints($form); if (!r.valid || !c.valid){ e.preventDefault(); self._updateErrorSummary($form, $.extend({}, r.errors, c.errors)); self._scrollToFirstError($form); return false; } }); },
        _initErrorSummary: function($form, selector){ var $s=$(selector); if ($s.length===0){ $s=$('<div data-error-summary style="display:none;"></div>'); $form.before($s);} $s.html('<h4>Please correct the following errors:</h4><ul id="error-list"></ul>'); },
        _updateErrorSummary: function($form, errors){ var $s=$('[data-error-summary]'); var $list=$s.find('#error-list'); $list.empty(); if (Object.keys(errors).length>0){ $.each(errors, function(field, messages){ $.each(messages, function(i,m){ $list.append('<li>'+field+': '+m+'</li>'); }); }); $s.show(); } else { $s.hide(); } },
        _scrollToFirstError: function($form){ var $e=$form.find('.is-invalid, .constraint-error').first(); if ($e.length>0){ $e[0].scrollIntoView({ behavior:'smooth', block:'center' }); $e.focus(); } }
    };
})(window, jQuery);

