/** Copied from frontend/src/js/composites.js for UI hosting */
(function(window, $) {
    'use strict';
    var registry = {};
    function collectDefinitions($root){
        $root.find('[data-composite-def]').each(function(){
            var $def=$(this), name=$def.attr('data-composite-def'); if (!name) return;
            var isContainer = $def.attr('data-container')==='true';
            var $designer = $def.find('[data-designer]').first();
            var $runtime = $def.find('[data-runtime]').first();
            var designerHtml = $designer.length ? $designer.html() : null;
            var runtimeHtml = $runtime.length ? $runtime.html() : null;
            var html = $def.html();
            registry[name]={ name:name, isContainer:isContainer, html:html, designerHtml: designerHtml || html, runtimeHtml: runtimeHtml || html };
            $def.attr('hidden', true).css('display','none');
        });
    }
    function renderInstances($root){
        $root.find('[data-composite]').each(function(){
            var $inst=$(this); if ($inst.data('compositeRendered')) return; var name=$inst.attr('data-composite'); var def=registry[name]; if (!def){ console.warn('[Composite] Missing def:', name); return; }
            var view = ($inst.attr('data-view')||'runtime').toLowerCase(); var html=(view==='designer'?def.designerHtml:def.runtimeHtml)||def.html||''; var props={}; $.each(this.attributes, function(){ var an=(this.name||'').toLowerCase(); if (an.startsWith('data-prop-')){ var pn=an.substring('data-prop-'.length); props[pn]=this.value||''; } });
            html=html.replace(/\{\{\s*prop:([\w-]+)\s*\}\}/g, function(_,p1){ return props[p1]||''; });
            $inst.html(html);
            $inst.find('[data-prop-bind]').each(function(){ var $el=$(this); var key=$el.attr('data-prop-bind'); var val=props[key]||''; if ($el.is('input,select,textarea')) $el.val(val); else $el.text(val); });
            wireEvents($inst, props);
            $inst.data('compositeRendered', true);
            dispatchMapped($inst, props, 'ready');
        });
    }
    function parseEventMap(attr){ var map=[]; if(!attr) return map; attr.split(';').forEach(function(rule){ rule=rule.trim(); if(!rule) return; var parts=rule.split('=>'); if(parts.length<1) return; var left=parts[0].trim(); var emit=(parts[1]||'').trim(); var ev=left, sel=null; var idx=left.indexOf(':'); if(idx>-1){ ev=left.substring(0,idx).trim(); sel=left.substring(idx+1).trim(); } if(!ev) return; map.push({on:ev, selector:sel, emit:emit}); }); return map; }
    function wireEvents($inst, props){ var spec=$inst.attr('data-events'); if(!spec) return; var mappings=parseEventMap(spec); var $form=$inst.closest('form'); mappings.forEach(function(m){ var $t=m.selector? $inst.find(m.selector):$inst; if ($t.length===0) return; $t.on(m.on, function(e){ var payload={ widgetId:$inst.attr('id')||'', widgetName:$inst.attr('data-composite')||'', emit:m.emit||'', props:props, target:e.currentTarget}; $form.trigger('formdsl:composite',[payload]); if(m.emit){ $form.trigger('formdsl:composite:'+m.emit,[payload]); } }); }); }
    function dispatchMapped($inst, props, eventName){ var spec=$inst.attr('data-events'); if(!spec) return; var mappings=parseEventMap(spec).filter(function(m){ return m.on.toLowerCase()===eventName; }); var $form=$inst.closest('form'); mappings.forEach(function(m){ var payload={ widgetId:$inst.attr('id')||'', widgetName:$inst.attr('data-composite')||'', emit:m.emit||'', props:props, target:$inst[0]}; $form.trigger('formdsl:composite',[payload]); if(m.emit){ $form.trigger('formdsl:composite:'+m.emit,[payload]); } }); }
    window.CompositeWidgets={
        initialize:function($form){ collectDefinitions($form); renderInstances($form); },
        isRegistered:function(n){return !!registry[n];},
        register:function(n,h,c){ registry[n]={name:n, html:h, isContainer:!!c}; },
        getState:function($root){ var state={}; $root.find('[data-composite]').each(function(){ var $inst=$(this); var id=$inst.attr('id'); if(!id) return; state[id]=getInstanceState($inst); }); return state; },
        setState:function($root, mapping){ if(!mapping) return; $root.find('[data-composite]').each(function(){ var $inst=$(this); var id=$inst.attr('id'); if(!id) return; var st=mapping[id]; if(!st) return; setInstanceState($inst, st); }); }
    };

    function getInstanceState($inst){ var props={}; $.each($inst[0].attributes, function(){ var an=(this.name||'').toLowerCase(); if(an.startsWith('data-prop-')){ var pn=an.substring('data-prop-'.length); props[pn]=this.value||''; } }); var data={}; $inst.find('[data-state-key]').each(function(){ var $el=$(this); var key=$el.attr('data-state-key'); if(!key) return; var val; if($el.is('input,select,textarea')) val=$el.val(); else val=$el.text(); data[key]=val; }); return { props:props, data:data}; }
    function setInstanceState($inst, state){ try{ if(state.props){ Object.keys(state.props).forEach(function(k){ $inst.attr('data-prop-'+k, state.props[k]); }); reRenderInstance($inst);} if(state.data){ $inst.find('[data-state-key]').each(function(){ var $el=$(this); var key=$el.attr('data-state-key'); if(!key) return; var val=state.data[key]; if(val===undefined) return; if($el.is('input,select,textarea')) $el.val(val).trigger('change'); else $el.text(val); }); } } catch(e){ console.error('[Composite] setInstanceState failed', e);} }
    function reRenderInstance($inst){ $inst.removeData('compositeRendered'); var parent=$('<div/>'); parent.append($inst); renderInstances(parent); parent.children().appendTo($inst.parent()); }
})(window, jQuery);
