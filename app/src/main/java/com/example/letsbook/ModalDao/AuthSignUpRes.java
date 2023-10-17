package com.example.letsbook.ModalDao;

public class AuthSignUpRes {

    private String data;
    private boolean success;
    private String message;

    public AuthSignUpRes(String data, boolean success, String message) {
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

    public boolean getSuccess() {
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
