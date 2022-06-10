import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Lightbox } from 'ngx-lightbox';
import { MediaItemsService } from '../../../media-items/services/media-items.service';
import { IGalleryImage } from '../../../models/media/gallery-image.interface';
import { IGroupMediaItem } from '../../../models/media/group-media-item.interface';
import { IMediaItemLink } from '../../../models/media/media-item-link.interface';
import { IMediaItem } from '../../../models/media/media-item.interface';
import { ImageSourceResolverService } from '../../../utility/image-source-resolver.serivce';

@Component({
  selector: 'app-library-item-details',
  templateUrl: './library-item-details.component.html',
  styleUrls: ['./library-item-details.component.scss']
})
export class LibraryItemDetailsComponent implements OnInit {

  public mediaItem!: IGroupMediaItem;
  public isLoading: boolean = true;
  public title: string = '';

  public columns = ['name', 'value']

  public hasItems: boolean = false;
  public hasImages: boolean = false;

  public images: IGalleryImage[] = [];
  public image!: IGalleryImage;

  private isChapter: boolean = false;

  constructor(
      private readonly router: Router,
      private readonly route: ActivatedRoute,
      private readonly mediaItemsService: MediaItemsService,
      private readonly lightbox: Lightbox,
      private readonly imageSourceResolverService: ImageSourceResolverService,
  ) { 
    const state = (this.router.getCurrentNavigation()?.extras?.state as any);
    this.isChapter = state?.isChapter ?? false;
  }

  public ngOnInit(): void {
    const routeParams = this.route.snapshot.paramMap;

    (
        this.isChapter
            ? this.mediaItemsService.getDetails(routeParams.get('id'))
            : this.mediaItemsService.getGroup(routeParams.get('id'))
    )
    .then(x => {
        this.mediaItem = x as IGroupMediaItem;

        if (this.mediaItem == null) {
          console.log('MediaItem is null');
          this.router.navigateByUrl('/');
        }

        this.title = this.isChapter ? this.mediaItem.groupingItem.chapterTitle : this.mediaItem.groupingItem.title;

        this.hasItems = this.mediaItem.items != null && this.mediaItem.items.length > 1;
        this.hasImages = this.mediaItem.groupingItem.images != null && this.mediaItem.groupingItem.images.length > 0;

        this.image = {
            id: this.mediaItem.groupingItem.image.id,
            filePath: this.mediaItem.groupingItem.image.image,
            src: this.imageSourceResolverService.resolve(this.mediaItem.groupingItem.image)
        };

        if (this.hasImages) {
            this.images = this.mediaItem
                .groupingItem
                .images
                .map(x => {
                    return {
                        id: x.id,
                        filePath: x.image,
                        src: this.imageSourceResolverService.resolve(x)
                    }
                });
        }
    })
    .catch((ex) => {
      console.log(ex);
      this.router.navigateByUrl('/');
    })
    .finally(() => this.isLoading = false)
    ;
  }

  public hasItemsInArray(arr: any[]): boolean {
    return arr && arr.length > 0;
  }

  public getDomainName(item: IMediaItemLink): string {
    return new URL(item.link).hostname;
  }

  public openImage(index: number): void {
    this.lightbox.open(this.images.concat([this.image]) as any, index);
  }

  public onBackClick(): void {
      const id = this.mediaItem.groupingItem.groupId;
      if (id != null) {
        this.router.navigateByUrl(`/details/${id}`);
      } else {
        this.router.navigateByUrl('/');
      }
  }
}
