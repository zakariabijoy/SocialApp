import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'users');
  }

  getMember(userName: string) {
    return this.http.get<Member>(this.baseUrl + 'users/' + userName);
  }

  updateMember(member: Member){
    return this.http.put(this.baseUrl + 'users', member);
  }
}
