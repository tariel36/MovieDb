import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-notification-email-dialog',
  templateUrl: './notification-email-dialog.component.html',
  styleUrls: ['./notification-email-dialog.component.scss']
})
export class NotificationEmailDialogComponent
    implements OnInit
{

  constructor(private readonly dialogRef: MatDialogRef<NotificationEmailDialogComponent>) { }

  public ngOnInit(): void {
  }

  public onCloseClicked(): void {
    this.dialogRef.close();
  }

}
