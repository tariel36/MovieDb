import { Component, Input, OnInit } from '@angular/core';
import { IMediaItem } from '../../../models/media/media-item.interface';

@Component({
  selector: 'app-collection',
  templateUrl: './collection.component.html',
  styleUrls: ['./collection.component.scss']
})
export class CollectionComponent {

    @Input() public items!: IMediaItem[]

    constructor() { }



}
