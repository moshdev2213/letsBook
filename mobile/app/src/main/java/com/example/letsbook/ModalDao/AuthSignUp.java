package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class AuthSignUp implements Serializable {
    private String phone;
    private String email;
    private String name;
    private String nic;
    private String password;
    private String passwordConfirm;

    public AuthSignUp(String phone, String email, String name, String nic, String password, String passwordConfirm) {
        this.phone = phone;
        this.email = email;
        this.name = name;
        this.nic = nic;
        this.password = password;
        this.passwordConfirm = passwordConfirm;
    }

    public String getPhone() {
        return phone;
    }

    public void setPhone(String phone) {
        this.phone = phone;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getNic() {
        return nic;
    }

    public void setNic(String nic) {
        this.nic = nic;
    }

    public String getPassword() {
        return password;
    }

    public void setPassword(String password) {
        this.password = password;
    }

    public String getPasswordConfirm() {
        return passwordConfirm;
    }

    public void setPasswordConfirm(String passwordConfirm) {
        this.passwordConfirm = passwordConfirm;
    }
}
