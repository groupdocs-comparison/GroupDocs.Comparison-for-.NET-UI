import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { APP_BASE_HREF } from '@angular/common';

import { AppComponent } from './app.component';
import { ConfigService } from '@groupdocs.examples.angular/common-components';
import { ComparisonModule } from "@groupdocs.examples.angular/comparison";

declare global {
  interface Window {
    apiEndpoint: string;
    uiSettingsPath: string;
  }
}

export function configServiceFactory() {
  let config = new ConfigService();
  config.apiEndpoint = window.apiEndpoint;
  config.getComparisonApiEndpoint = () => window.apiEndpoint;
  config.getConfigEndpoint = () => window.uiSettingsPath;
  return config;
}

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule,
    ComparisonModule],
    providers: [
      { provide: APP_BASE_HREF, useValue: '/' },
      { provide: ConfigService, useFactory: configServiceFactory },
    ],
  bootstrap: [AppComponent]
})
export class AppModule {}
