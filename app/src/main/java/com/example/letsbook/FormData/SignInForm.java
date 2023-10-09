package com.example.letsbook.FormData;

import com.example.letsbook.Validation.ValidationResult;

public class SignInForm {
    private String tel;
    private String email;
    private String password;

    public SignInForm(String email,String password) {
        this.email = email;
        this.password = password;
    }

    public ValidationResult validateTel() {
        String telPattern = "^\\d{10}$";

        if (tel.isEmpty()) {
            return new ValidationResult.Empty("Please Enter Number");
        } else if (!tel.matches(telPattern)) {
            return new ValidationResult.Invalid("Please Enter Valid Number");
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }
    public ValidationResult validatePassword(String password) {
        // The regex pattern
        String passwordPattern = "^(?=.*[a-zA-Z])(?=.*\\d)[a-zA-Z\\d]{8,}$";

        if (password.isEmpty()) {
            return new ValidationResult.Empty("Enter Password");
        } else if (!password.matches(passwordPattern)) {
            return new ValidationResult.Invalid("Invalid ex: Aa@asda22");
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }

    public ValidationResult validateEmail(String email) {
        // The regex pattern
        String regex = "[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}";

        if (email.isEmpty()) {
            return new ValidationResult.Empty("Enter Email");
        } else if (!email.matches(regex)) {
            return new ValidationResult.Invalid("Enter Valid Email");
        } else {
            return ValidationResult.Valid.getInstance();
        }
    }

}
