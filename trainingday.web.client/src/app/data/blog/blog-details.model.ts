export class BlogDetails {
    id : number;
    title : string;
    description : string;
    author : string;
    image : string;
    date : Date;
    view: string
    constructor (id : number, title : string, description : string, author : string, image : string, date : Date, view:string) {
        this.id = id;
        this.title = title;
        this.description = description;
        this.author = author;
        this.image = image;
        this.date = date;
        this.view = view;
    }
}
