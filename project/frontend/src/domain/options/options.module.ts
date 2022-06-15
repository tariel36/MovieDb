import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OptionsContainerComponent } from './components/options-container/options-container.component';
import { MaterialModule } from '../../material/material.module';
import { LanguagesModule } from '../languages/languages.module';
import { TranslateModule } from '@ngx-translate/core';
import { OptionsContainerDialogComponent } from './components/options-container-dialog/options-container-dialog.component';

@NgModule({
  declarations: [
    OptionsContainerComponent,
    OptionsContainerDialogComponent
  ],
  imports: [
    CommonModule,
    MaterialModule,
    LanguagesModule,
    TranslateModule
  ],
  exports: [
    OptionsContainerComponent,
    OptionsContainerDialogComponent
  ]
})
export class OptionsModule { }
