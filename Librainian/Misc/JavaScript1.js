﻿(function () {
    var f = !0, g = null, i = !1, aa = function (a, b, c) { return a.call.apply(a.bind, arguments); }, ba = function (a, b, c) {
        if (!a) throw Error();
        if (2 < arguments.length) {
            var e = Array.prototype.slice.call(arguments, 2);
            return function () {
                var c = Array.prototype.slice.call(arguments);
                Array.prototype.unshift.apply(c, e);
                return a.apply(b, c);
            };
        }
        return function () { return a.apply(b, arguments); };
    }, j = function (a, b, c) {
        j = Function.prototype.bind && -1 != Function.prototype.bind.toString().indexOf("native code") ? aa : ba;
        return j.apply(g, arguments);
    };
    var k = (new Date).getTime();
    var ca = /&/g, da = /</g, ea = />/g, fa = /\"/g, m = { "\x00": "\\0", "\b": "\\b", "\f": "\\f", "\n": "\\n", "\r": "\\r", "\t": "\\t", "\x0B": "\\x0B", '"': '\\"', "\\": "\\\\" }, p = { "'": "\\'" };
    var q = window, s, ga = g, t = document.getElementsByTagName("script");
    t && t.length && (ga = t[t.length - 1].parentNode);
    s = ga;
    var ha = function (a) {
        a = parseFloat(a);
        return isNaN(a) || 1 < a || 0 > a ? 0 : a;
    }, ia = /^([\w-]+\.)*([\w-]{2,})(\:[0-9]+)?$/, u = function (a) { return !a ? "pagead2.googlesyndication.com" : (a = a.match(ia)) ? a[0] : "pagead2.googlesyndication.com"; };
    u("");
    var v = function (a) { return !!a && "function" == typeof a && !!a.call; }, ja = function (a, b) { if (!(2 > arguments.length)) for (var c = 1, e = arguments.length; c < e; ++c) a.push(arguments[c]); };

    function w(a) { return "function" == typeof encodeURIComponent ? encodeURIComponent(a) : escape(a); }

    var ka = function (a, b) {
        if (!(1E-4 > Math.random())) {
            var c = Math.random();
            if (c < b) return a[Math.floor(c / b * a.length)];
        }
        return g;
    }, y = function (a) {
        try {
            return !!a.location.href || "" === a.location.href;
        } catch (b) {
            return i;
        }
    };
    var z = g, la = function () {
        if (!z) {
            for (var a = window, b = a, c = 0; a != a.parent;)
                if (a = a.parent, c++, y(a)) b = a;
                else break;
            z = b;
        }
        return z;
    };
    var A, B = function (a) {
        this.c = [];
        this.a = a || window;
        this.b = 0;
        this.d = g;
    }, ma = function (a, b) {
        this.l = a;
        this.i = b;
    };
    B.prototype.n = function (a, b) { 0 == this.b && 0 == this.c.length && (!b || b == window) ? (this.b = 2, this.g(new ma(a, window))) : this.h(a, b); };
    B.prototype.h = function (a, b) {
        this.c.push(new ma(a, b || this.a));
        C(this);
    };
    B.prototype.o = function (a) {
        this.b = 1;
        a && (this.d = this.a.setTimeout(j(this.f, this), a));
    };
    B.prototype.f = function () {
        1 == this.b && (this.d != g && (this.a.clearTimeout(this.d), this.d = g), this.b = 0);
        C(this);
    };
    B.prototype.p = function () { return f; };
    B.prototype.nq = B.prototype.n;
    B.prototype.nqa = B.prototype.h;
    B.prototype.al = B.prototype.o;
    B.prototype.rl = B.prototype.f;
    B.prototype.sz = B.prototype.p;
    var C = function (a) { a.a.setTimeout(j(a.m, a), 0); };
    B.prototype.m = function () {
        if (0 == this.b && this.c.length) {
            var a = this.c.shift();
            this.b = 2;
            a.i.setTimeout(j(this.g, this, a), 0);
            C(this);
        }
    };
    B.prototype.g = function (a) {
        this.b = 0;
        a.l();
    };
    var na = function (a) {
        try {
            return a.sz();
        } catch (b) {
            return i;
        }
    }, oa = function (a) { return !!a && ("object" == typeof a || "function" == typeof a) && na(a) && v(a.nq) && v(a.nqa) && v(a.al) && v(a.rl); }, D = function () {
        if (A && na(A)) return A;
        var a = la(), b = a.google_jobrunner;
        return oa(b) ? A = b : a.google_jobrunner = A = new B(a);
    }, pa = function (a, b) { D().nq(a, b); }, qa = function (a, b) { D().nqa(a, b); };
    var ra = /MSIE [2-7]|PlayStation|Gecko\/20090226/i, sa = /Android|Opera/, ta = function () {
        var a = E, b = F.google_ad_width, c = F.google_ad_height, e = ["<iframe"], d;
        for (d in a) a.hasOwnProperty(d) && ja(e, d + "=" + a[d]);
        e.push('style="left:0;position:absolute;top:0;"');
        e.push("></iframe>");
        b = "border:none;height:" + c + "px;margin:0;padding:0;position:relative;visibility:visible;width:" + b + "px";
        return ['<ins style="display:inline-table;', b, '"><ins id="', a.id + "_anchor", '" style="display:block;', b, '">', e.join(" "), "</ins></ins>"].join("");
    };
    var ua = /^true$/.test("false") ? f : i;
    var va = function (a, b, c) {
        c || (c = ua ? "https" : "http");
        return [c, "://", a, b].join("");
    };
    var wa = function () {
    }, G = function (a, b, c) {
        switch (typeof b) {
            case "string":
                xa(b, c);
                break;
            case "number":
                c.push(isFinite(b) && !isNaN(b) ? b : "null");
                break;
            case "boolean":
                c.push(b);
                break;
            case "undefined":
                c.push("null");
                break;
            case "object":
                if (b == g) {
                    c.push("null");
                    break;
                }
                if (b instanceof Array) {
                    var e = b.length;
                    c.push("[");
                    for (var d = "", h = 0; h < e; h++) c.push(d), G(a, b[h], c), d = ",";
                    c.push("]");
                    break;
                }
                c.push("{");
                e = "";
                for (d in b) b.hasOwnProperty(d) && (h = b[d], "function" != typeof h && (c.push(e), xa(d, c), c.push(":"), G(a, h, c), e = ","));
                c.push("}");
                break;
            case "function":
                break;
            default:
                throw Error("Unknown type: " + typeof b);
        }
    }, K = { '"': '\\"', "\\": "\\\\", "/": "\\/", "\b": "\\b", "\f": "\\f", "\n": "\\n", "\r": "\\r", "\t": "\\t", "\x0B": "\\u000b" }, za = /\uffff/.test("\uffff") ? /[\\\"\x00-\x1f\x7f-\uffff]/g : /[\\\"\x00-\x1f\x7f-\xff]/g, xa = function (a, b) {
        b.push('"');
        b.push(a.replace(za, function (a) {
            if (a in K) return K[a];
            var b = a.charCodeAt(0), d = "\\u";
            16 > b ? d += "000" : 256 > b ? d += "00" : 4096 > b && (d += "0");
            return K[a] = d + b.toString(16);
        }));
        b.push('"');
    };
    var L = "google_ad_block google_ad_channel google_ad_client google_ad_format google_ad_height google_ad_host google_ad_host_channel google_ad_host_tier_id google_ad_output google_ad_override google_ad_region google_ad_section google_ad_slot google_ad_type google_ad_width google_adtest google_allow_expandable_ads google_alternate_ad_url google_alternate_color google_analytics_domain_name google_analytics_uacct google_bid google_city google_color_bg google_color_border google_color_line google_color_link google_color_text google_color_url google_container_id google_contents google_country google_cpm google_ctr_threshold google_cust_age google_cust_ch google_cust_gender google_cust_id google_cust_interests google_cust_job google_cust_l google_cust_lh google_cust_u_url google_disable_video_autoplay google_ed google_eids google_enable_ose google_encoding google_font_face google_font_size google_frame_id google_gl google_hints google_image_size google_kw google_kw_type google_language google_max_num_ads google_max_radlink_len google_mtl google_num_radlinks google_num_radlinks_per_unit google_num_slots_to_rotate google_only_ads_with_video google_only_pyv_ads google_only_userchoice_ads google_override_format google_page_url google_previous_watch google_previous_searches google_referrer_url google_region google_reuse_colors google_rl_dest_url google_rl_filtering google_rl_mode google_rt google_safe google_scs google_skip google_tag_info google_targeting google_tdsma google_tfs google_tl google_ui_features google_ui_version google_video_doc_id google_video_product_type google_with_pyv_ads google_yt_pt google_yt_up".split(" ");
    var M = function (a) {
        this.a = a;
        a.google_iframe_oncopy || (a.google_iframe_oncopy = { handlers: {}, log: [], shouldLog: 0.01 > Math.random() ? f : i });
        this.e = a.google_iframe_oncopy;
        a.setTimeout(j(this.k, this), 3E4);
    }, Aa;
    var N = "var i=this.id,s=window.google_iframe_oncopy,H=s&&s.handlers,h=H&&H[i],w=this.contentWindow,d;try{d=w.document}catch(e){}if(h&&d&&(!d.body||!d.body.firstChild)){if(h.call){i+='.call';setTimeout(h,0)}else if(h.match){i+='.nav';w.location.replace(h)}s.log&&s.log.push(i)}";
    /[&<>\"]/.test(N) && (-1 != N.indexOf("&") && (N = N.replace(ca, "&amp;")), -1 != N.indexOf("<") && (N = N.replace(da, "&lt;")), -1 != N.indexOf(">") && (N = N.replace(ea, "&gt;")), -1 != N.indexOf('"') && (N = N.replace(fa, "&quot;")));
    Aa = N;
    M.prototype.set = function (a, b) {
        this.e.handlers[a] = b;
        this.a.addEventListener && this.a.addEventListener("load", j(this.j, this, a), i);
    };
    M.prototype.j = function (a) {
        var a = this.a.document.getElementById(a), b = a.contentWindow.document;
        if (a.onload && b && (!b.body || !b.body.firstChild)) a.onload();
    };
    M.prototype.k = function () {
        if (this.e.shouldLog) {
            var a = this.e.log, b = this.a.document;
            if (a.length) {
                b = ["/pagead/gen_204?id=iframecopy&log=", w(a.join("-")), "&url=", w(b.URL.substring(0, 512)), "&ref=", w(b.referrer.substring(0, 512))].join("");
                a.length = 0;
                a = this.a;
                b = va(u(""), b);
                a.google_image_requests || (a.google_image_requests = []);
                var c = a.document.createElement("img");
                c.src = b;
                a.google_image_requests.push(c);
            }
        }
    };
    var Ba = function () {
        var a = "script";
        return ["<", a, ' src="', va(u(""), "/pagead/js/r20120725/r20120730/show_ads_impl.js", ""), '"></', a, ">"].join("");
    }, Ca = function (a, b, c, e) {
        return function () {
            var d = i;
            e && D().al(3E4);
            try {
                if (y(a.document.getElementById(b).contentWindow)) {
                    var h = a.document.getElementById(b).contentWindow, o = h.document;
                    if (!o.body ||
                        !o.body.firstChild) o.open(), h.google_async_iframe_close = f, o.write(c);
                } else {
                    var H = a.document.getElementById(b).contentWindow, W;
                    h = c;
                    h = String(h);
                    if (h.quote) W = h.quote();
                    else {
                        for (var o = ['"'], I = 0; I < h.length; I++) {
                            var J = h.charAt(I), ya = J.charCodeAt(0), eb = o, fb = I + 1, X;
                            if (!(X = m[J])) {
                                var x;
                                if (31 < ya && 127 > ya) x = J;
                                else {
                                    var n = J;
                                    if (n in p) x = p[n];
                                    else if (n in m) x = p[n] = m[n];
                                    else {
                                        var l = n, r = n.charCodeAt(0);
                                        if (31 < r && 127 > r) l = n;
                                        else {
                                            if (256 > r) {
                                                if (l = "\\x", 16 > r || 256 < r) l += "0";
                                            } else l = "\\u", 4096 > r && (l += "0");
                                            l += r.toString(16).toUpperCase();
                                        }
                                        x =
                                            p[n] = l;
                                    }
                                }
                                X = x;
                            }
                            eb[fb] = X;
                        }
                        o.push('"');
                        W = o.join("");
                    }
                    H.location.replace("javascript:" + W);
                }
                d = f;
            } catch (nb) {
                H = la().google_jobrunner, oa(H) && H.rl();
            }
            d && (new M(a)).set(b, Ca(a, b, c, i));
        };
    }, Da = Math.floor(1E6 * Math.random()), Ea = function (a) {
        for (var a = a.data.split("\n"), b = {}, c = 0; c < a.length; c++) {
            var e = a[c].indexOf("=");
            -1 != e && (b[a[c].substr(0, e)] = a[c].substr(e + 1));
        }
        b[1] == Da && (window.google_top_url = b[3]);
    };
    var Fa = ha("0.001"), Ga = ha("0.001");
    window.google_loader_used = f;
    var O = window;
    if (!("google_onload_fired" in O)) {
        O.google_onload_fired = i;
        var Ha = function () { O.google_onload_fired = f; };
        O.addEventListener ? O.addEventListener("load", Ha, i) : O.attachEvent && O.attachEvent("onload", Ha);
    }
    var P = window, Q = 2;
    try {
        P.top.document == P.document ? Q = 0 : y(P.top) && (Q = 1);
    } catch (Ia) {
    }
    if (2 === Q && top.postMessage && !window.google_top_experiment && (window.google_top_experiment = ka(["jp_e", "jp_c"], Ga), "jp_e" === window.google_top_experiment)) {
        var R = window;
        R.addEventListener ? R.addEventListener("message", Ea, i) : R.attachEvent && R.attachEvent("onmessage", Ea);
        var Ja = { "0": "google_loc_request", 1: Da }, Ka = [], S;
        for (S in Ja) Ka.push(S + "=" + Ja[S]);
        top.postMessage(Ka.join("\n"), "*");
    }
    window.google_adk_experiment || (window.google_adk_experiment = ka(["expt", "control"], Fa) || "none");
    var La;
    if (window.google_enable_async === i) La = 0;
    else {
        var Ma = navigator.userAgent;
        La = (ra.test(Ma) || sa.test(Ma) ? i : f) && !window.google_container_id && (!window.google_ad_output || "html" == window.google_ad_output);
    }
    if (La) {
        var Na = window;
        Na.google_unique_id ? ++Na.google_unique_id : Na.google_unique_id = 1;
        var T = window;
        if (!T.google_slot_list || !T.google_slot_list.push) T.google_slot_list = [];
        T.google_slot_list.push([T.google_ad_slot || "", T.google_ad_format || "", T.google_ad_width || "", T.google_ad_height || ""].join("."));
        for (var F = window, script$$Inline89 = "script", U, E = {
            allowtransparency: '"true"',
            frameborder: '"0"',
            height: '"' + F.google_ad_height + '"',
            hspace: '"0"',
            marginwidth: '"0"',
            marginheight: '"0"',
            onload: '"' + Aa + '"',
            scrolling: '"no"',
            vspace: '"0"',
            width: '"' + F.google_ad_width + '"'
        }, Oa = F.document, V = E.id, Pa = 0; !V || F.document.getElementById(V) ;) V = "aswift_" + Pa++;
        E.id = V;
        E.name = V;
        Oa.write(ta());
        U = V;
        var Qa, Ra = F.google_adk_experiment, Sa;
        if ("control" == Ra) Sa = "control";
        else {
            var Ta;
            if ("expt" == Ra) {
                var Ua = [q.google_ad_slot, q.google_ad_format, q.google_ad_type, q.google_ad_width, q.google_ad_height];
                if (s) {
                    var Y;
                    if (s) {
                        for (var Va = [], Wa = 0, Z = s; Z && 25 > Wa; Z = Z.parentNode, ++Wa) Va.push(9 != Z.nodeType && Z.id || "");
                        Y = Va.join();
                    } else Y = "";
                    Y && Ua.push(Y);
                }
                var Xa = 0;
                if (Ua) {
                    var Ya =
                            Ua.join(":"), Za = Ya.length;
                    if (0 == Za) Xa = 0;
                    else {
                        for (var $ = 305419896, $a = 0; $a < Za; $a++) $ ^= ($ << 5) + ($ >> 2) + Ya.charCodeAt($a) & 4294967295;
                        Xa = 0 < $ ? $ : 4294967296 + $;
                    }
                }
                Ta = Xa.toString();
            } else Ta = g;
            Sa = Ta;
        }
        Qa = Sa;
        var ab;
        F.google_page_url && (F.google_page_url = String(F.google_page_url));
        for (var bb = [], cb = 0, db = L.length; cb < db; cb++) {
            var gb = L[cb];
            if (F[gb] != g) {
                var hb;
                try {
                    var ib = [];
                    G(new wa, F[gb], ib);
                    hb = ib.join("");
                } catch (jb) {
                }
                hb && ja(bb, gb, "=", hb, ";");
            }
        }
        ab = bb.join("");
        for (var kb = 0, lb = L.length; kb < lb; kb++) F[L[kb]] = g;
        var mb = (new Date).getTime(),
            ob = window.google_top_experiment, pb = ["<!doctype html><html><body><", script$$Inline89, ">", ab, "google_show_ads_impl=true;google_unique_id=", F.google_unique_id, ';google_async_iframe_id="', U, '";google_start_time=', k, ";", ob ? 'google_top_experiment="' + ob + '";' : "", Qa ? 'google_adk_sa="' + Qa + '";' : "", "google_bpp=", mb > k ? mb - k : 1, ";</", script$$Inline89, ">", Ba(), "</body></html>"].join("");
        (F.document.getElementById(U) ? pa : qa)(Ca(F, U, pb, f));
    } else window.q = k, document.write(Ba());
})();