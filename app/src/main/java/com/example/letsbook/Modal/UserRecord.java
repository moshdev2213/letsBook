package com.example.letsbook.Modal;

import com.example.letsbook.ModalDao.Record;

import java.io.Serializable;

public class UserRecord implements Serializable {

    private String data;
    private boolean success;
    private String message;

    public UserRecord(String data, boolean success, String message) {
        this.data = data;
        this.success = success;
        this.message = message;
    }

    public String getData() {
        return data;
    }

    public void setData(String data) {
        this.data = data;
    }

    public boolean isSuccess() {
        return success;
    }

    public void setSuccess(boolean success) {
        this.success = success;
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }
}
