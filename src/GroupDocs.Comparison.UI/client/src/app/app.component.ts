import { Component, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ComparisonAppComponent, ComparisonService, ComparisonConfigService, DifferencesService } from '@groupdocs.examples.angular/comparison';
import { Api, ModalService, UploadFilesService, TabActivatorService, PagePreloadService, PasswordService, TypedFileCredentials, ConfigService } from '@groupdocs.examples.angular/common-components';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less', './variables.less']
})
export class AppComponent extends ComparisonAppComponent {

    apiConfigService: ConfigService;
    comparisonService: ComparisonService;
    http: HttpClient;

    constructor(comparisonService: ComparisonService,
      comparisonConfigService: ComparisonConfigService,
      differencesService: DifferencesService,
      uploadFilesService: UploadFilesService,
      pagePreloadService: PagePreloadService,
      modalService: ModalService,
      tabActivatorService: TabActivatorService,
      elementRef: ElementRef<HTMLElement>,
      passwordService: PasswordService, 
      http: HttpClient,
      apiConfigService: ConfigService) {

          super(comparisonService,
            comparisonConfigService,
            differencesService,
            uploadFilesService,
            pagePreloadService,
            modalService,
            tabActivatorService,
            elementRef,
            passwordService);
  
          this.comparisonService = comparisonService;
          this.http = http;
          this.apiConfigService = apiConfigService;
      }
      
      loadPage(credentials: TypedFileCredentials, pages: number[]) {
        return this.http.post(this.apiConfigService.getComparisonApiEndpoint() + Api.LOAD_DOCUMENT_PAGE, {
            'Guid': credentials.guid,
            'FileType': credentials.fileType,
            'Password': credentials.password,
            'Page': pages
        }, Api.httpOptionsJson);
    }
}    