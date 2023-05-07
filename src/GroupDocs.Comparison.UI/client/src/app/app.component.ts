import { Component, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ComparisonAppComponent, ComparisonService, ComparisonConfigService, DifferencesService } from '@groupdocs.examples.angular/comparison';
import { ModalService, UploadFilesService, TabActivatorService, PagePreloadService, PasswordService } from '@groupdocs.examples.angular/common-components';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less', './variables.less']
})
export class AppComponent extends ComparisonAppComponent {

    comparisonService: ComparisonService;
    http: HttpClient;

    constructor(comparisonService: ComparisonService,
      configService: ComparisonConfigService,
      differencesService: DifferencesService,
      uploadFilesService: UploadFilesService,
      pagePreloadService: PagePreloadService,
      modalService: ModalService,
      tabActivatorService: TabActivatorService,
      elementRef: ElementRef<HTMLElement>,
      passwordService: PasswordService, 
      http: HttpClient) {

          super(comparisonService,
            configService,
            differencesService,
            uploadFilesService,
            pagePreloadService,
            modalService,
            tabActivatorService,
            elementRef,
            passwordService);
  
          this.comparisonService = comparisonService;
          this.http = http;
      }
}    