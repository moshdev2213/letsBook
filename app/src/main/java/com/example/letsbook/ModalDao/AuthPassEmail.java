package com.example.letsbook.ModalDao;

public class AuthPassEmail {
    private String identity;
    private String password;

    public AuthPassEmail(String identity, String password) {
        this.identity = identity;
        this.password = password;
    }

    public String getIdentity() {
        return identity;
    }

    public String getPassword() {
        return password;
    }
}
