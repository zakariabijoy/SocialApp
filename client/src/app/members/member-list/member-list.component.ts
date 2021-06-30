import { Component, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/_services/members.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  members$: Observable<Member[]>;

  constructor(private memeberService: MembersService) {}

  ngOnInit(): void {
    this.members$ = this.memeberService.getMembers();
  }
}
