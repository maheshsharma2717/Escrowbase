import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { XmlHttpRequestHelper } from '@shared/helpers/XmlHttpRequestHelper';
import { LocalStorageService } from '@shared/utils/local-storage.service';

@Injectable()
export class AppAuthService {
    fileid: string;

    logout(reload?: boolean, returnUrl?: string): void {
          
        let customHeaders = {
            [abp.multiTenancy.tenantIdCookieName]: abp.multiTenancy.getTenantIdCookie(),
            'Authorization': 'Bearer ' + abp.auth.getToken()
        };

        XmlHttpRequestHelper.ajax(
            'GET',
            AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/LogOut', customHeaders, null, () => {
                  
               
                    localStorage.removeItem('OpenTabList');
                    localStorage.clear();
                abp.auth.clearToken();
                abp.auth.clearRefreshToken();
              
                new LocalStorageService().removeItem(AppConsts.authorization.encrptedAuthTokenName,
                    () => {
                         
                        if (reload !== false) {
                            if (returnUrl) {
                                location.href = returnUrl;
                            } else {
                                location.href = '';
                            }
                        }
                    }
                );
            }
        );
          localStorage.removeItem('activeTab');
          localStorage.removeItem('OpenTabList');
        //  localStorage.setItem('homeOpened','false'); 
        //  location.href = '';
    }
}
