import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { InstructionsComponent } from './components/instructions/instructions.component';
import { TranslateModule } from '@ngx-translate/core';
import { MaterialModule } from '../../material/material.module';



@NgModule({
  declarations: [InstructionsComponent],
  imports: [
    CommonModule,
    TranslateModule,
    MaterialModule
  ],
  exports: [InstructionsComponent]
})
export class MediaItemsModule { }
