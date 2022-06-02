import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { MediaItemsService } from '../../../media-items/services/media-items.service';
import { LocalStorageKeys } from '../../../utility/local-storage-keys.enum';
import { LocalStorageService } from '../../../utility/local-storage.service';

@Component({
  selector: 'app-notification-email',
  templateUrl: './notification-email.component.html',
  styleUrls: ['./notification-email.component.scss']
})
export class NotificationEmailComponent
    implements OnInit
{
    @Input() public isCancelVisible: boolean = true;
    @Output() public closeClicked: EventEmitter<void> = new EventEmitter<void>();

    public email: string = '';

    constructor(
        private readonly localStorageService: LocalStorageService,
        private readonly mediaItemsService: MediaItemsService
    ) { }

    public ngOnInit(): void {
        this.email = this.localStorageService.getValue(LocalStorageKeys.notificationEmail) ?? '';
    }

    public onEmailInput(args: any): void {
        this.email = (args?.target as any)?.value ?? '';
    }

    public onApplyClicked(): void {
        const types = JSON.parse(this.localStorageService.getValue(LocalStorageKeys.selectedMediaItemTypes) ?? '[]') as number[];
        this.localStorageService.setValue(LocalStorageKeys.notificationEmail, this.email);
        this.localStorageService.setValue(LocalStorageKeys.isNotificationSelected, "1");
        this.mediaItemsService.setNotificationEmail(this.email, types);
        
        this.closeClicked?.emit();
    }

    public onCancelClicked(): void {
        this.localStorageService.setValue(LocalStorageKeys.isNotificationSelected, "1");
        this.closeClicked?.emit();
    }
}
