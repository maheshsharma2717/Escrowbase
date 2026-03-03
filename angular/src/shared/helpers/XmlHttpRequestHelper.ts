export class XmlHttpRequestHelper {

    /**
     * Lightweight XHR wrapper used during pre-bootstrap (before Angular HttpClient is ready).
     * IMPORTANT: This is used for critical startup calls; it must fail fast and not hang forever.
     */
    static ajax(
        type: string,
        url: string,
        customHeaders: any,
        data: any,
        success: (result: any) => void,
        error?: (err: { url: string; status?: number; statusText?: string; responseText?: string; kind: 'http' | 'timeout' | 'network' }) => void,
        timeoutMs: number = 60000
    ) {
        let xhr = new XMLHttpRequest();

        xhr.timeout = timeoutMs;

        xhr.onreadystatechange = () => {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    let result = JSON.parse(xhr.responseText);
                    success(result);
                } else if (xhr.status !== 0) {
                    if (error) {
                        error({
                            url,
                            status: xhr.status,
                            statusText: xhr.statusText,
                            responseText: xhr.responseText,
                            kind: 'http'
                        });
                    } else {
                        alert(abp.localization.localize('InternalServerError', 'AbpWeb'));
                    }
                }
            }
        };

        xhr.ontimeout = () => {
            if (error) {
                error({ url, kind: 'timeout' });
            } else {
                alert('Request timed out.');
            }
        };

        xhr.onerror = () => {
            if (error) {
                error({ url, kind: 'network' });
            } else {
                alert('Network error.');
            }
        };

        url += (url.indexOf('?') >= 0 ? '&' : '?') + 'd=' + new Date().getTime();
        xhr.open(type, url, true);

        for (let property in customHeaders) {
            if (customHeaders.hasOwnProperty(property)) {
                xhr.setRequestHeader(property, customHeaders[property]);
            }
        }

        xhr.setRequestHeader('Content-type', 'application/json');
        if (data) {
            xhr.send(data);
        } else {
            xhr.send();
        }
    }
}
