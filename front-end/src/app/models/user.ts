export class User {
    id: String;
    name!: String;
    surname!: String;
    username!: String

    constructor(id: String, name: String, surname: String, username: String) {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.username = username;
    }
}