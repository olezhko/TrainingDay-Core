export class BlogPreview {
  id: number;
  title: string;
  description: string;
  author: string;
  image: string;
  date: Date;
  tags: string[] = [];

  constructor(id: number, title: string, description: string, author: string, image: string, date: Date, tags: string[]) {
    this.id = id;
    this.title = title;
    this.description = description;
    this.author = author;
    this.image = image;
    this.date = date;
    this.tags = tags;
  }
}

export class BlogPostEditViewModel {
  id!: number; // blogpost-culture connection
  cultureId!: number;
  blogId!: number; // blogpost id
  title!: string;
  description!: string;
  date!: Date;
  author!: string;
  view!: string;
  tags: string[] = [];
}
