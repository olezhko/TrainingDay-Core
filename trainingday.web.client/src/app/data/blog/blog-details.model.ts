export class BlogDetails {
    id : number;
    title : string;
    description : string;
    author : string;
    image : string;
    date : Date;

    constructor (id : number, title : string, description : string, author : string, image : string, date : Date) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.author = author;
        this.image = image;
        this.date = date;
    }
}