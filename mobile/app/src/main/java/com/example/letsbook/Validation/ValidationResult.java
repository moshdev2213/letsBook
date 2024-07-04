package com.example.letsbook.Validation;

public abstract class ValidationResult {

    public static final class Empty extends ValidationResult {
        private final String errorMsg;

        public Empty(String errorMsg) {
            this.errorMsg = errorMsg;
        }

        public String getErrorMsg() {
            return errorMsg;
        }
    }

    public static final class Invalid extends ValidationResult {
        private final String errorMsg;

        public Invalid(String errorMsg) {
            this.errorMsg = errorMsg;
        }

        public String getErrorMsg() {
            return errorMsg;
        }
    }

    public static final class Valid extends ValidationResult {
        private Valid() {
        }

        public static Valid getInstance() {
            return Holder.INSTANCE;
        }

        private static class Holder {
            private static final Valid INSTANCE = new Valid();
        }
    }
}
