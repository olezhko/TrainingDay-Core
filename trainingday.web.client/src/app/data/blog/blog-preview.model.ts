export class BlogPreview {
    id : number;
    title : string;
    shortDescription : string;
    author : string;
    image : string;
    date : Date;
    
    constructor(id : number, title : string, shortDescription : string, author : string, image : string, date : Date) {
        this.id = id;
        this.title = title;
        this.shortDescription = shortDescription;
        this.author = author;
        this.image = image;
        this.date = date;
    } 
}