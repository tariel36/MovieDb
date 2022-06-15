import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';
import { OptionsContainerComponent } from '../options-container/options-container.component';

@Component({
  selector: 'app-options-container-dialog',
  templateUrl: './options-container-dialog.component.html',
  styleUrls: ['./options-container-dialog.component.scss']
})
export class OptionsContainerDialogComponent
{
  constructor(private readonly dialogRef: MatDialogRef<OptionsContainerComponent>) { }
}
