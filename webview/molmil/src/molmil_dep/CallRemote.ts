namespace molmil_dep {

    interface XDomainRequest extends XMLHttpRequest {
        XHRO;
        CRO: CallRemote;
        ERROR: boolean;
        loadHandler;
        silent;
    }

    export class CallRemote {

        formData;
        request: XDomainRequest;
        Method;
        parameters: string;
        headers: {};
        ASYNC: boolean;
        ctype: string;
        responseType;
        forceSSL: boolean;
        URL;

        constructor(method, crossDomain = false) {
            this.formData = null; this.request = null; this.Method = method; this.parameters = <any>[], this.headers = {};
            if (method == "POSTv2" && typeof (FormData) != "undefined") { this.formData = new FormData(); }
            else if (method == "POSTv2") { this.Method = "POST"; }
            this.ASYNC = false;
            this.ctype = "application/x-www-form-urlencoded";
            this.responseType = null;
            this.forceSSL = false;
            this.initRequest(crossDomain);
        }

        initRequest(crossDomain) {
            var stupidIE = molmil_dep.dBT.MSIE && molmil_dep.dBT.majorVersion < 10;
            if (crossDomain && molmil_dep.dBT.MSIE && molmil_dep.dBT.majorVersion < 10 && typeof (XDomainRequest) != "undefined") {
                stupidIE = false;
                this.request = new XDomainRequest();
                this.Method = "POST";
            }
            else this.request = <any>new XMLHttpRequest();
            this.request.XHRO = this.request;
            var ref = this.request.CRO = this;// firefox 2.0
            this.request.ERROR = false;
            this.request.loadHandler = function () {
                if (this.CRO.VirtualOnDone) { this.CRO.VirtualOnDone(); }
                if (this.status && this.status > 399) { this.ERROR = true; }
                if (this.CRO.OnDone) {
                    try { if (!this.ERROR) this.CRO.OnDone(); } catch (e) { this.ERROR = e; }
                    if (this.ERROR) {
                        if (this.CRO.OnError) this.CRO.OnError();
                        else { if (!this.silent) throw this.ERROR; }
                    }
                }
            };
            if (this.request.onload == null && !stupidIE) {
                this.request.onload = this.request.loadHandler
            }
            else this.request.onreadystatechange = function () {
                if (this.readyState != 4) { return false; }
                this.loadHandler();
            };
            if (molmil_dep.dBT.MSIE && typeof (XDomainRequest) != "undefined" && this instanceof XDomainRequest) this.request.onload = this.request.onreadystatechange;
        };

        AddParameter(variable, value) {
            if (this.Method == "POSTv2") { this.formData.append(variable, value); }
            else { this.parameters += (this.parameters.length ? "&" : "") + variable + "=" + encodeURIComponent(value); }
        };

        AddParameters(list) {
            for (var i = 0; i < list.length; i++) this.AddParameter(list[i][0], list[i][1]);
        };

        Send(URL, silent) {
            URL = URL || this.URL;
            if (this.forceSSL) URL = URL.replace("http://", "https://");
            if (molmil_dep.dBT.MSIE && molmil_dep.dBT.majorVersion < 10) {
                var domain = URL.split("/");
                for (var i = 0; i < domain.length; i++) if (domain[i].indexOf(".") != -1) { domain = domain[i]; break; }
                if (domain != document.domain) this.initRequest(true);
            }
            if (molmil_dep.dBT.denyASYNC) { this.ASYNC = false; }
            // use withCredentials...
            if (this.Method == "GET") {
                this.request.open("GET", URL + (this.parameters.length ? "?" + this.parameters : ""), this.ASYNC);
                if (this.responseType) { this.request.responseType = this.responseType; }
                this.request.send(null);
            }
            else if (this.Method == "POST") {
                this.request.open("POST", URL, this.ASYNC);
                try { this.request.setRequestHeader("Content-Type", this.ctype); }
                catch (e) { };// stupid IE
                for (var e in this.headers) {
                    try { this.request.setRequestHeader(e, this.headers[e]); }
                    catch (e) { }; //stupid IE
                }
                if (this.responseType) { this.request.responseType = this.responseType; }
                this.request.send((this.parameters.length ? this.parameters : null));
            }
            else if (this.Method == "POSTv2" && this.formData) {
                this.request.open("POST", URL, this.ASYNC);
                if (this.responseType) { this.request.responseType = this.responseType; } this.request.send(this.formData);
            }
            else if (this.Method == "HEAD") { this.request.open("HEAD", URL, this.ASYNC); this.request.send(null); }
            //  if (this.request.ERROR) {throw "ERROR";}
            this.request.silent = silent;
        };

        getURL(URL) {
            return URL + (this.parameters.length ? "?" + this.parameters : "");
        }
    }
}