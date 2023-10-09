package com.example.letsbook.ModalDao;

public class AuthSignUpRes {
    private String id;
    private String email;
    private String name;
    private String nic;
    private int phone;


    public AuthSignUpRes(String id, String email, String name, String nic, int phone) {
        this.id = id;
        this.email = email;
        this.name = name;
        this.nic = nic;
        this.phone = phone;
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

    public String getNic() {
        return nic;
    }

    public void setNic(String nic) {
        this.nic = nic;
    }

    public int getPhone() {
        return phone;
    }

    public void setPhone(int phone) {
        this.phone = phone;
    }
}
