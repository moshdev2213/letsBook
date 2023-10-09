package com.example.letsbook.FormData;

import com.example.letsbook.Validation.ValidationResult;

public class SignUpform {
    private String tel;
    private String email;
    private String password;
    private String nic;
    private String name;

    public SignUpform(String tel, String email, String name, String nic,String password) {
        this.tel = tel;
        this.email = email;
        this.password = password;
        this.nic = nic;
        this.name = name;
    }

    public ValidationResult validateEmail(String email) {
        String regex = "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}"; // Regex to match email addresses

        if (email.isEmpty()) {
            return new ValidationResult.Empty("Please Enter Email");
        } else if (email.equals(password)) {
            return new ValidationResult.Invalid("Email Shouldn't be password");
        } else if (!email.matches(regex)) {
            return new ValidationResult.Invalid("Please Enter Valid Email");
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }

    public ValidationResult validatePassword(String password) {
        String passwordPattern = "^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$"; // Regex to validate password

        if (password.isEmpty()) {
            return new ValidationResult.Empty("Please Enter Password");
        } else if (!password.matches(passwordPattern)) {
            return new ValidationResult.Invalid("Invalid ex: Aa@asda22"); // Password is invalid
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }

    public ValidationResult validateTelephone(String tel) {
        String telPattern = "^\\d{10}$"; // Regex to validate telephone number

        if (tel.isEmpty()) {
            return new ValidationResult.Empty("Please Enter Telephone");
        } else if (!tel.matches(telPattern)) {
            return new ValidationResult.Invalid("Invalid ex: 0765654332"); // Telephone is invalid
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }

    public ValidationResult validateNic(String nic) {
        String nicPattern = "^(?:[0-9]{8}v|[0-9]+)$";// Regex to validate telephone number

        if (nic.isEmpty()) {
            return new ValidationResult.Empty("Please NIC");
        } else if (!tel.matches(nicPattern)) {
            return new ValidationResult.Invalid("Invalid ex: 200643231243"); // Telephone is invalid
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }
    public ValidationResult validateName(String name) {
        String namePattern = "^[a-zA-Z ]+$";// Regex to validate telephone number

        if (name.isEmpty()) {
            return new ValidationResult.Empty("Please Enter Name");
        } else if (!name.matches(namePattern)) {
            return new ValidationResult.Invalid("Invalid ex: moshdev"); // Telephone is invalid
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }
}
