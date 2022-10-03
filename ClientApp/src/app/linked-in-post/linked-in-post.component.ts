import { Component } from '@angular/core';
import {HttpClient} from "@angular/common/http";

@Component({
  selector: 'app-linked-in-post',
  templateUrl: './linked-in-post.component.html',
  styleUrls: ['./linked-in-post.component.scss']
})



export class PostOnLinkedIn {
  defaultText: string = "Default Text on LinkedIn Post!";
  text: string = this.defaultText;
  code: string;

  constructor(private http: HttpClient) {
    this.code = location.search
  }

  onTextChange({event}: { event: any }) {
    this.text = event;
  }

  onClick() {
    this.http.post('https://localhost:7000/linkedin/post' + this.code, {text: this.text}).subscribe(
      (res) => {console.log(res)},
      (error) => console.log(error));
  }
}

