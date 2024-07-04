package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class Record implements Serializable {
    private String email;

    public Record(String email) {
        this.email = email;
    }

    public String getEmail() {
        return email;
    }
}
