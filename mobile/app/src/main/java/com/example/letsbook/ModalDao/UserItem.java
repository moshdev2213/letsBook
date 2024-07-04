package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class UserItem implements Serializable {
    private String email;
    private String id;
    private String name;
    private long phone;
    private String nic;
    private boolean verified;

    public UserItem(String email, String id, String name, long phone, String nic, boolean verified) {
        this.email = email;
        this.id = id;
        this.name = name;
        this.phone = phone;
        this.nic = nic;
        this.verified = verified;
    }

    public String getNic() {
        return nic;
    }

    public void setNic(String nic) {
        this.nic = nic;
    }

    public boolean getVerified() {
        return verified;
    }

    public void setVerified(boolean verified) {
        this.verified = verified;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
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

    public long getPhone() {
        return phone;
    }

    public void setPhone(long phone) {
        this.phone = phone;
    }
}