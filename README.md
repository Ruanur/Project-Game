# Master Kitchen

<div align="center">
<img width="820" alt="image" src="https://github.com/user-attachments/assets/cbd53e91-7280-4748-975e-3b099b771d3f">

[![Hits](https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2FVoluntain-SKKU%2FVoluntain-2nd&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&title=hits&edge_flat=false)](https://hits.seeyoufarm.com)

</div>

# Master Kitchen
> **인디유 게임 공모전 출품작** <br/> **개발기간: 2023.04 ~ 2023.10**

## 배포 주소

> **개발 버전** : [https://drive.google.com/file/d/11qh8j_F4D8HitKmhPnFbc_ntenko1TZJ/view?usp=sharing] <br>
> **빌드 버전(실행 파일)** : [https://drive.google.com/file/d/1yyOvpkJi79B3B6D3dxK0-SjxNj_lOqet/view?usp=sharing]<br>

## 개발팀 소개

|      민경진       |                                                                                                                 
| :------------------------------------------------------------------------------: | 
|   <img width="200px" src="https://github.com/user-attachments/assets/3c97d6ba-902b-4607-a309-34ea32b5cf05" />    | 
|   [@Ruanur](https://github.com/Ruanur)   | 
| 공주대학교 소프트웨어학과 4학년 |

## 프로젝트 소개

Master Kitchen은 플레이어가 주문을 받아 요리를 서빙하는 게임입니다.
OverCooked! 게임을 친구들와 인상깊게 플레이했던 경험이 있었는데, 게임 가격이 조금 부담이였던 친구들도 있어 같이 게임을 즐기지 못한 것에 아쉬움을 느껴
이 게임을 통해 같이 모두가 즐겼으면 좋겠다는 생각에 Master Kitchen을 만들게 되었습니다.

플레이어가 직접 재료를 손질하고, 패티를 굽고, 접시에 플레이팅 하여 제한시간안에 더 많은 음식을 서빙하는 것이 게임 내에서의 목표입니다.

<img width="780" alt="image" src="https://github.com/user-attachments/assets/8945b432-69e2-46ad-aeb9-2a0907b260e2">

## 시작 가이드
### Requirements
For building and running the application you need:

- [Unity Editor](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe)
- [Unity 2022.2.12f1](https://unity.com/kr/releases/editor/archive)

### Installation
``` bash
$ git clone https://github.com/Ruanur/Project-Game.git
$ cd Master-Kitchen
```

---

## Stacks 🐈

### Environment
![Visual Studio 2022](https://img.shields.io/badge/Visual%20Studio%202022-007ACC?style=for-the-badge&logo=Visual%20Studio%20Code&logoColor=white)
![Github Desktop](https://img.shields.io/badge/Github%20Desktop-C363F7?style=for-the-badge&logo=GitHub&logoColor=purple)
![Github](https://img.shields.io/badge/GitHub-181717?style=for-the-badge&logo=GitHub&logoColor=white)             
      
### Development
![C#](https://img.shields.io/badge/C%20Sharp-512BD4?style=flat-square&logo=sharp&logoColor=white)
![Unity](https://img.shields.io/badge/Unity-E6E6FA?style=for-the-badge&logo=unity&logoColor=black)

---
## 인게임 구성 📺
| 메인 화면  |   주방 스테이지   |
| :-------------------------------------------: | :------------: |
|  <img width="329" src="https://github.com/user-attachments/assets/20b18985-f96d-430f-a04a-a7ff7b836e30"/> |  <img width="329" src="https://github.com/user-attachments/assets/e6c948cc-3bb9-4932-9b78-6d44e7bdc514"/>|  
| 손질 재료 - 양상추, 토마토, 치즈 |  조리 재료 - 패티  |  
| <img width="329" src="https://github.com/user-attachments/assets/0abc507f-280d-4de6-8a5e-ab839950f89c"/>   |  <img width="329" src="https://github.com/user-attachments/assets/b8dad125-c262-4cc2-9a85-28dcc6473b4d"/>     |
| 서빙 성공   |  서빙 실패   |  
| <img width="329" src="https://github.com/user-attachments/assets/21c91b96-898c-4a9e-8a72-767a1ad5bb64"/>   |  <img width="329" src="https://github.com/user-attachments/assets/b2a089f0-a513-47ea-bbdc-28cf8bc540d8"/>     |
| 멀티플레이 화면   |  캐릭터 선택 화면   |  
| <img width="329" src="https://github.com/user-attachments/assets/34d16b2e-db3e-4e20-82a4-66ae7fd45e5f"/>   |  <img width="329" src="https://github.com/user-attachments/assets/7200d5b0-461d-4aaf-9b22-ea5a8134ecf3"/>     |
| 시간 초과, 게임 오버 |
| <img width="329" src="https://github.com/user-attachments/assets/2516a3fa-e854-42e5-a422-8f56fed32dc0"/>   
---
## 게임 내 구조 📦

<img width="329" src="https://github.com/user-attachments/assets/0f29abad-dd0b-4e44-8b24-5be7f0791737"/>

게임의 전체적인 구조는 다음과 같습니다.
멀티플레이 씬과 싱글플레이 씬이 있으며, 내부의 UI를 통해 각 씬의 상태에 따라 게임이 진행됩니다.

---

## 주요 기능 📦

### ⭐️ 강좌 선택 및 강의 영상 시청 기능
- Scratch, Python 2개 강좌 및 각 강좌마다 10개 가량의 강의 영상 제공
- 추후 지속적으로 강좌 추가 및 업로드 예정

### ⭐️ 강의 관련 및 단체에 대한 자유로운 댓글 작성 가능
- Disqus를 이용하여 강의 관련 질문이나 단체에 대한 질문 작성 가능

### ⭐️ 이어 학습하기 기능
- Cookie 기능을 이용하여 이전에 학습했던 내용 이후부터 바로 학습 가능

---
## 아키텍쳐

### 디렉토리 구조
```bash
├── README.md
├── package-lock.json
├── package.json
├── strapi-backend : 
│   ├── README.md
│   ├── api : db model, api 관련 정보 폴더
│   │   ├── about
│   │   ├── course
│   │   └── lecture
│   ├── config : 서버, 데이터베이스 관련 정보 폴더
│   │   ├── database.js
│   │   ├── env : 배포 환경(NODE_ENV = production) 일 때 설정 정보 폴더
│   │   ├── functions : 프로젝트에서 실행되는 함수 관련 정보 폴더
│   │   └── server.js
│   ├── extensions
│   │   └── users-permissions : 권한 정보
│   ├── favicon.ico
│   ├── package-lock.json
│   ├── package.json
│   └── public
│       ├── robots.txt
│       └── uploads : 강의 별 사진
└── voluntain-app : 프론트엔드
    ├── README.md
    ├── components
    │   ├── CourseCard.js
    │   ├── Footer.js
    │   ├── LectureCards.js
    │   ├── MainBanner.js : 메인 페이지에 있는 남색 배너 컴포넌트, 커뮤니티 이름과 슬로건을 포함.
    │   ├── MainCard.js
    │   ├── MainCookieCard.js
    │   ├── NavigationBar.js : 네비게이션 바 컴포넌트, _app.js에서 공통으로 전체 페이지에 포함됨.
    │   ├── RecentLecture.js
    │   └── useWindowSize.js
    ├── config
    │   └── next.config.js
    ├── lib
    │   ├── context.js
    │   └── ga
    ├── next.config.js
    ├── package-lock.json
    ├── package.json
    ├── pages
    │   ├── _app.js
    │   ├── _document.js
    │   ├── about.js
    │   ├── course
    │   ├── index.js
    │   ├── lecture
    │   ├── newcourse
    │   ├── question.js
    │   └── setting.js
    ├── public
    │   ├── favicon.ico
    │   └── logo_about.png
    └── styles
        └── Home.module.css

```

<!--
```bash
├── README.md : 리드미 파일
│
├── strapi-backend/ : 백엔드
│   ├── api/ : db model, api 관련 정보 폴더
│   │   └── [table 이름] : database table 별로 분리되는 api 폴더 (table 구조, 해당 table 관련 api 정보 저장)
│   │       ├── Config/routes.json : api 설정 파일 (api request에 따른 handler 지정)
│   │       ├── Controllers/ [table 이름].js : api controller 커스텀 파일
│   │       ├── Models : db model 관련 정보 폴더
│   │       │   ├── [table 이름].js : (사용 X) api 커스텀 파일
│   │       │   └── [table 이름].settings.json : model 정보 파일 (field 정보)
│   │       └─── Services/ course.js : (사용 X) api 커스텀 파일
│   │ 
│   ├── config/ : 서버, 데이터베이스 관련 정보 폴더
│   │   ├── Env/production : 배포 환경(NODE_ENV = production) 일 때 설정 정보 폴더
│   │   │   └── database.js : production 환경에서 database 설정 파일
│   │   ├── Functions : 프로젝트에서 실행되는 함수 관련 정보 폴더
│   │   │   │   ├── responses : (사용 X) 커스텀한 응답 저장 폴더
│   │   │   │   ├── bootstrap.js : 어플리케이션 시작 시 실행되는 코드 파일
│   │   │   │   └── cron.js : (사용 X) cron task 관련 파일
│   │   ├── database.js : 기본 개발 환경(NODE_ENV = development)에서 database 설정 파일
│   │   └── server.js : 서버 설정 정보 파일
│   │  
│   ├── extensions/
│   │   └── users-permissions/config/ : 권한 정보
│   │ 
│   └── public/
│       └── uploads/ : 강의 별 사진
│
└── voluntain-app/ : 프론트엔드
    ├── components/
    │   ├── NavigationBar.js : 네비게이션 바 컴포넌트, _app.js에서 공통으로 전체 페이지에 포함됨.
    │   ├── MainBanner.js : 메인 페이지에 있는 남색 배너 컴포넌트, 커뮤니티 이름과 슬로건을 포함.
    │   ├── RecentLecture.js : 사용자가 시청 정보(쿠키)에 따라, 현재/다음 강의를 나타내는 컴포넌트 [호출: MainCookieCard]
    │   ├── MainCookieCard.js : 상위 RecentLecture 컴포넌트에서 전달받은 props를 나타내는 레이아웃 컴포넌트.
    │   ├── MainCard.js : 현재 등록된 course 정보를 백엔드에서 받아서 카드로 나타내는 컴포넌트 [호출: CourseCard]
    │   └── CourseCard.js : 상위 MainCard 컴포넌트에서 전달받은 props를 나타내는 레이아웃 컴포넌트
    │
    ├── config/
    │   └── next.config.js
    │
    ├── lib/
    │   └── ga/
    │   │   └── index.js
    │   └── context.js
    │
    ├── pages/
    │   ├── courses/
    │   │   └── [id].js : 강의 페이지
    │   ├── _app.js : Next.js에서 전체 컴포넌트 구조를 결정, 공통 컴포넌트(navbar, footer)가 선언되도록 customizing 됨.
    │   ├── _document.js : Next.js에서 전체 html 문서의 구조를 결정, lang 속성과 meta tag가 customizing 됨.
    │   ├── about.js : 단체 소개 페이지
    │   ├── index.js : 메인 페이지
    │   ├── question.js : Q&A 페이지
    │   └── setting.js : 쿠키, 구글 애널리틱스 정보 수집 정책 페이지
    │
    ├── public/
    │   ├── favicon.ico : 네비게이션바 이미지
    │   └── logo_about.png : about 페이지 로고 이미지
    │
    └── styles/
        └── Home.module.css

```
-->
