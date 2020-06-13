create table Users
(
	id_user int IDENTITY( 1 , 1 ) PRIMARY KEY,
	email varchar(max) not null,
	phone varchar(11) not null,
	user_hesh varchar(max) not null ,
	user_token varchar(max) not null
)

create table RequestPOS
(
	id_req int IDENTITY( 1 , 1 ) PRIMARY KEY,
	id_user int not null,
	text_request varchar(max),
	inn varchar(max) not null,
	address varchar(max),
	status varchar(max),
	CONSTRAINT [FK_RequestPOS_Users] FOREIGN KEY (id_user) REFERENCES Users (id_user) ON DELETE CASCADE ON UPDATE CASCADE

)

create table RequestProd
(
	id_req int IDENTITY( 1 , 1 ) PRIMARY KEY,
	id_user int not null,
	text_request varchar(max),
	barcode varchar(max),
	status varchar(max),
	CONSTRAINT [FK_RequestProd_Users] FOREIGN KEY (id_user) REFERENCES Users (id_user) ON DELETE CASCADE ON UPDATE CASCADE
)

create table FeedbackPOS
(
	id_feed int IDENTITY( 1 , 1 ) PRIMARY KEY,
	id_req int not null,
	feedback_text varchar(max) not null,
	CONSTRAINT [FK_FeedbackPOS_RequestPOS] FOREIGN KEY (id_req) REFERENCES RequestPOS (id_req ) ON DELETE CASCADE ON UPDATE CASCADE

)

create table FeedbackProd
(
	id_feed int IDENTITY( 1 , 1 ) PRIMARY KEY,
	id_req int,
	feedback_text varchar(max) not null,
	CONSTRAINT [FK_FeedbackProd_RequestPOS] FOREIGN KEY (id_req) REFERENCES RequestProd (id_req ) ON DELETE CASCADE ON UPDATE CASCADE
)

create table UserAdmin
(
	id_user int IDENTITY( 1 , 1 ) PRIMARY KEY,
	login varchar(max) not null,
	password varchar(max) not null,
	token varchar(max) not null
)