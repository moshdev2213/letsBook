package com.example.letsbook.ModalDao;

public class AuthPassEmail {
    private String email;
    private String password;

    public AuthPassEmail(String identity, String password) {
        this.email = identity;
        this.password = password;
    }

    public String getIdentity() {
        return email;
    }

    public String getPassword() {
        return password;
    }
}
