import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppComponent } from './app.component';
import { APP_BASE_HREF } from '@angular/common';
import { ComparisonModule } from '@groupdocs.examples.angular/comparison';

declare global {
  interface Window {
    apiEndpoint: string;
    uiSettingsPath: string;
  }
}

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    ComparisonModule
  ],
  providers: [
    { provide: APP_BASE_HREF, useValue: '/' },
    { provide: 'WINDOW', useValue: window }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
