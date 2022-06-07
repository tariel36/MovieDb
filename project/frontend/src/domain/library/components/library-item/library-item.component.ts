import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IMediaItem } from '../../../models/media/media-item.interface';

@Component({
  selector: 'app-library-item',
  templateUrl: './library-item.component.html',
  styleUrls: ['./library-item.component.scss']
})
export class LibraryItemComponent implements OnInit {

    @Input() public mediaItem!: IMediaItem;
    @Input() public isChapter: boolean = false;

    public title: string = '';
    public hasMultipleItems = false;

    constructor(private readonly router: Router) { }
    
    public ngOnInit(): void {
        this.title = this.isChapter && this.mediaItem.chapterTitle
            ? this.mediaItem.chapterTitle
            : this.mediaItem.title
            ;

        this.hasMultipleItems = this.mediaItem.itemsCount > 1;
    }

    public onClick() {
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.navigateByUrl(`/details/${this.mediaItem.id}`, { state: { isChapter: this.isChapter }});
    }
}
