create table Users
(
	id_user int IDENTITY( 1 , 1 ) PRIMARY KEY,
	email varchar(max) not null,
	phone varchar(11) not null,
	user_hesh varchar(max) not null ,
	user_token varchar(max) not null
)

create table Request
(
	id int IDENTITY( 1 , 1 ) PRIMARY KEY,
	id_user int not null,
	text_request varchar(max),
	address varchar(max),
	status varchar(max),
	unit varchar(max),
	type varchar(max),
	CONSTRAINT [FK_Request_Users] FOREIGN KEY (id_user) REFERENCES Users (id_user) ON DELETE CASCADE ON UPDATE CASCADE
)

create table Feedback
(
	id_feed int IDENTITY( 1 , 1 ) PRIMARY KEY,
	id_req int not null,
	feedback_text varchar(max) not null,
	CONSTRAINT [FK_Feedback_Request] FOREIGN KEY (id_req) REFERENCES Request (id) ON DELETE CASCADE ON UPDATE CASCADE
)

create table UserAdmin
(
	id_user int IDENTITY( 1 , 1 ) PRIMARY KEY,
	login varchar(max) not null,
	password varchar(max) not null,
	token varchar(max) not null
)