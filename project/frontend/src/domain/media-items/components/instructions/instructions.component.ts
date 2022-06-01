import { Component, Input, OnInit } from '@angular/core';
import { Lightbox } from 'ngx-lightbox';

@Component({
  selector: 'app-instructions',
  templateUrl: './instructions.component.html',
  styleUrls: ['./instructions.component.scss']
})
export class InstructionsComponent
    implements OnInit
{
    @Input() public instructions?: string

    public images: { src: string }[] = []
    public selectedInstructions?: IInstructions;
    public instructionsRows: IInstructions[] = []

    constructor(
        private readonly lightbox: Lightbox
    ) { 
    }

    public ngOnInit(): void {
        this.images = [
            'assets/instructions/generic-01-a.png',
            'assets/instructions/generic-01-b.png',
            'assets/instructions/generic-02-a.png',
            'assets/instructions/generic-02-a.png',
            'assets/instructions/generic-03-a.png',
            'assets/instructions/generic-01-a.png',
            'assets/instructions/generic-01-b.png',
            'assets/instructions/generic-02-a.png',
            'assets/instructions/generic-02-a.png',
            'assets/instructions/generic-03-a.png',
            'assets/instructions/anime-01-a.png',
            'assets/instructions/bluray-01-a.png',
            'assets/instructions/bluray-02-a.png',
            'assets/instructions/bluray-02-b.png',
            'assets/instructions/bluray-03-a.png',
            'assets/instructions/bluray-03-b.png',
            'assets/instructions/bluray-04-a.png',
            'assets/instructions/bluray-04-b.png',
            'assets/instructions/bluray-05-a.png',
            'assets/instructions/bluray-05-b.png',
            'assets/instructions/bluray-06-a.png',
            'assets/instructions/bluray-06-b.png',
        ].map(x => { return { src: x }; });

        this.instructionsRows = [
            {
                type: 'generic',
                rows: [
                    { text: 'instructions-generic-1', images: [0, 1]  },
                    { text: 'instructions-generic-2', images: [2, 3]  },
                    { text: 'instructions-generic-3', images: [4]  },
                ]
            },
            {
                type: 'anime',
                rows: [
                    { text: 'instructions-generic-1', images: [0, 1]  },
                    { text: 'instructions-generic-2', images: [2, 3]  },
                    { text: 'instructions-generic-3', images: [4]  },
                    { text: 'instructions-anime-1', images: [5]  }
                ]
            },
            {
                type: 'bdvm',
                rows: [
                    { text: 'instructions-bluray-1', images: [6]  },
                    { text: 'instructions-bluray-2', images: [7, 8]  },
                    { text: 'instructions-bluray-3', images: [9, 10]  },
                    { text: 'instructions-bluray-4', images: [11, 12]  },
                    { text: 'instructions-bluray-5', images: [13, 14]  },
                    { text: 'instructions-bluray-6', images: [15, 16]  },
                    { text: 'instructions-generic-3', images: [4]  },
                    { text: 'instructions-anime-1', images: [5]  }
                ]
            },
        ];

        this.selectedInstructions = this.instructionsRows.find(x => x.type == this.instructions);
    }

    public openImage(index: number): void {
        this.lightbox.open(this.images as any, index);
    }
}

interface IInstructions {
    type: string;
    rows: {
        text: string;
        images: number[];
    }[]
}