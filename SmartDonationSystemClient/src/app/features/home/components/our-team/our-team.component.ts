import { NgFor } from '@angular/common';
import { Component } from '@angular/core';

@Component({
  selector: 'app-our-team',
  standalone: true,
  imports: [NgFor],
  templateUrl: './our-team.component.html',
  styleUrl: './our-team.component.scss',
})
export class OurTeamComponent {
  teamMembers = [
    {
      name: 'Carol Younan',
      role: 'Front End (React)',
      bio: 'Expert in building immersive, accessible, and high-performance user interfaces.',
      image: './assets/our-team/carol.jpg',
    },
    {
      name: 'Micheal Ghobrial',
      role: 'Full Stack (.NET & Angular)',
      bio: 'Passionate about transparent giving and leveraging blockchain for social good.',
      image: './assets/our-team/micheal.jpeg',
    },
    {
      name: 'Kerolos Tamer',
      role: 'Back End (Node.js)',
      bio: 'Focused on architecting scalable server-side systems and secure API integrations.',
      image: './assets/our-team/kerolos.jpg',
    },
    {
      name: 'Felopater Sherif',
      role: 'Back End (Node.js)',
      bio: 'Specialized in database optimization and building robust backend infrastructures.',
      image: './assets/our-team/felopater.png',
    },
    {
      name: 'Youstina Adel',
      role: 'UI/UX Designer',
      bio: 'Creating intuitive and emotionally resonant design systems for social impact.',
      image: './assets/our-team/youstina.jpg',
    },
    {
      name: 'Pola Soliman',
      role: 'QA / QC Engineer',
      bio: 'Ensuring the highest standards of software quality and seamless user experiences.',
      image: './assets/our-team/pola.jpg',
    },
  ];
}
