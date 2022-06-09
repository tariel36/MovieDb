import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IGalleryImage } from '../../../models/media/gallery-image.interface';
import { IMediaItem } from '../../../models/media/media-item.interface';
import { ImageSourceResolverService } from '../../../utility/image-source-resolver.serivce';

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
    public image!: IGalleryImage;

    constructor(
        private readonly router: Router,
        private readonly imageSourceResolverService: ImageSourceResolverService,
    ) { }
    
    public ngOnInit(): void {
        this.title = this.isChapter && this.mediaItem.chapterTitle
            ? this.mediaItem.chapterTitle
            : this.mediaItem.title
            ;

        this.hasMultipleItems = this.mediaItem.itemsCount > 1;

        this.image = {
            id: this.mediaItem.image.id,
            filePath: this.mediaItem.image.image,
            src: this.imageSourceResolverService.resolve(this.mediaItem.image)
        }
    }

    public onClick() {
        this.router.routeReuseStrategy.shouldReuseRoute = () => false;
        this.router.navigateByUrl(`/details/${this.mediaItem.id}`, { state: { isChapter: this.isChapter }});
    }
}
