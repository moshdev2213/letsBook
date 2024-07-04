package com.example.letsbook.Modal;

import com.example.letsbook.ModalDao.Record;

import java.io.Serializable;

public class UserRecord implements Serializable {
    private String token;
    private Record record;

    public UserRecord(String token, Record record) {
        this.token = token;
        this.record = record;
    }

    public String getToken() {
        return token;
    }

    public Record getRecord() {
        return record;
    }
}
