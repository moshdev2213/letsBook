package com.example.letsbook.FormData;

import com.example.letsbook.Validation.ValidationResult;

public class SignInForm {
    private String tel;

    public SignInForm(String tel) {
        this.tel = tel;
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
}
