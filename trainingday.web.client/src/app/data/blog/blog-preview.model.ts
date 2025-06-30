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

export interface BlogPostEditViewModel {
  title: string;
  content: string;
  culture: string;
}
