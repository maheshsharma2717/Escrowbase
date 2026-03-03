import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { LocalStorageService } from '@shared/utils/local-storage.service';

@Injectable()
export class AppAuthService {
    fileid: string;

    logout(reload?: boolean, returnUrl?: string): void {

        // 🔹 Save keys you want to keep
        const sortField = localStorage.getItem('otherSortField');
        const sortOrder = localStorage.getItem('otherSortOrder');

        let customHeaders = {
            [abp.multiTenancy.tenantIdCookieName]: abp.multiTenancy.getTenantIdCookie(),
            'Authorization': 'Bearer ' + abp.auth.getToken()
        };

        XmlHttpRequestHelper.ajax(
            'GET',
            AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/LogOut',
            customHeaders,
            null,
            () => {

                // ❌ REMOVE only what you WANT to remove
                localStorage.removeItem('OpenTabList');

                // ❌ Avoid localStorage.clear() — remove only what you need
                // localStorage.clear();  // REMOVE THIS LINE

                abp.auth.clearToken();
                abp.auth.clearRefreshToken();

                new LocalStorageService().removeItem(
                    AppConsts.authorization.encrptedAuthTokenName,
                    () => {

                        // 🔹 Restore your sort values
                        if (sortField !== null) localStorage.setItem('otherSortField', sortField);
                        if (sortOrder !== null) localStorage.setItem('otherSortOrder', sortOrder);

                        if (reload !== false) {
                            location.href = returnUrl || '';
                        }
                    }
                );
            }
        );

        localStorage.removeItem('activeTab');
        localStorage.removeItem('OpenTabList');
    }

}
