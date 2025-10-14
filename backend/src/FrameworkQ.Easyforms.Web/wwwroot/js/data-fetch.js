/**
 * Copied from frontend/src/js/data-fetch.js for UI hosting
 */
(function(window, $) {
    'use strict';
    window.DataFetchModule = {
        _cache: {},
        initialize: function($form, options){ var settings=$.extend({ apiBaseUrl:'/api', fetchTimeout:5000, fetchRetries:2 }, options); var self=this; $form.find('[data-fetch]').each(function(){ var $field=$(this); var cfg=self._parseFetchConfig($field); $field.data('fetchConfig', cfg); self._bindFetchEvents($field, $form, cfg, settings); if (cfg.fetchOn.indexOf('load')!==-1){ self.fetchOptions($field, $form, settings);} }); this._bindCascadeDependencies($form, settings); },
        fetchOptions: function($field, $form, settings){ var cfg=$field.data('fetchConfig'); if(!cfg) return $.Deferred().reject('No config').promise(); var url=this._substituteTokens(cfg.url, $form, $field); var key=this._getCacheKey(url, cfg.cache); if (key && this._cache[key]){ this._populateOptions($field, this._cache[key], cfg.map); return $.Deferred().resolve(this._cache[key]).promise(); }
            var proxyUrl=settings.apiBaseUrl + '/v1/proxy/fetch'; var params={ endpoint:url, method: cfg.method }; var self=this; return $.ajax({ url:proxyUrl, method:'GET', data:params, timeout: settings.fetchTimeout })
                .done(function(data){ if (key) self._cache[key]=data; self._populateOptions($field, data, cfg.map); })
                .fail(function(xhr){ console.error('[DataFetch] Fetch failed', xhr); }); },
        _parseFetchConfig: function($field){ var df=$field.attr('data-fetch')||''; var parts=df.split(':',2); return { method: parts.length>1?parts[0]:'GET', url: parts.length>1?parts[1]:parts[0], fetchOn: ($field.attr('data-fetch-on')||'focus').split(','), minChars: parseInt($field.attr('data-min-chars'))||0, debounceMs: parseInt(($field.attr('data-fetch-debounce')||'').replace('ms',''))||300, map:$field.attr('data-fetch-map'), cache:$field.attr('data-fetch-cache')||'session', depends: ($field.attr('data-depends')||'').split(',').map(function(s){return s.trim();}) || [] } },
        _bindFetchEvents: function($field, $form, cfg, settings){ var self=this; cfg.fetchOn.forEach(function(event){ event=event.trim(); if (event==='focus'){ $field.on('focus', function(){ self.fetchOptions($field, $form, settings); }); } else if (event==='input' || event==='change'){ var timer; $field.on(event, function(){ clearTimeout(timer); timer=setTimeout(function(){ var v=$field.val(); if (v && v.length >= cfg.minChars){ self.fetchOptions($field, $form, settings); } }, cfg.debounceMs); }); } }); },
        _bindCascadeDependencies: function($form, settings){ var self=this; $form.find('[data-depends]').each(function(){ var $field=$(this); var depends=$field.attr('data-depends').split(','); depends.forEach(function(sel){ sel=sel.trim(); var $parent=$(sel); $parent.on('change', function(){ $field.val(''); self.fetchOptions($field, $form, settings); }); }); }); },
        _substituteTokens: function(url, $form, $field){ var result=url; var tokens=url.match(/\{([^}]+)\}/g) || []; tokens.forEach(function(t){ var name=t.slice(1,-1); var val=''; if (name==='search' || name==='value') val=$field.val()||''; else { var $src=$form.find('[name="'+name+'"]'); val=$src.val()||''; } result=result.replace(t, encodeURIComponent(val)); }); return result; },
        _populateOptions: function($field, data, mapCfg){ if (!Array.isArray(data)){ console.warn('[DataFetch] Expected array, got:', typeof data); return; } var map={ value:'value', label:'label' }; if (mapCfg){ mapCfg.split(',').forEach(function(p){ var kv=p.split(':'); if (kv.length===2){ map[kv[0].trim()]=kv[1].trim(); } }); }
            if ($field.is('select')){ var cur=$field.val(); $field.empty(); $field.append('<option value="">-- Select --</option>'); data.forEach(function(item){ var value=item[map.value]||item.value||item.id; var label=item[map.label]||item.label||item.name||value; $field.append('<option value="'+value+'">'+label+'</option>'); }); if (cur) $field.val(cur); }
        },
        _getCacheKey: function(url, cacheMode){ if (cacheMode==='none') return null; if (cacheMode==='session' || (cacheMode||'').startsWith('ttl:')) return 'fetch:'+url; return null; }
    };
})(window, jQuery);

