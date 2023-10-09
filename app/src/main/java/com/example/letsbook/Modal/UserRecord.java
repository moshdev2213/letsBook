package com.example.letsbook.Modal;

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
class Record implements Serializable {
    private String id;
    private String collectionId;
    private String collectionName;
    private String username;
    private String description;
    private boolean verified;
    private boolean emailVisibility;
    private String email;
    private String created;
    private String updated;
    private String name;
    private String telephone;
    private String avatar;

    public Record(
            String id,
            String collectionId,
            String collectionName,
            String username,
            String description,
            boolean verified,
            boolean emailVisibility,
            String email,
            String created,
            String updated,
            String name,
            String telephone,
            String avatar
    ) {
        this.id = id;
        this.collectionId = collectionId;
        this.collectionName = collectionName;
        this.username = username;
        this.description = description;
        this.verified = verified;
        this.emailVisibility = emailVisibility;
        this.email = email;
        this.created = created;
        this.updated = updated;
        this.name = name;
        this.telephone = telephone;
        this.avatar = avatar;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getCollectionId() {
        return collectionId;
    }

    public void setCollectionId(String collectionId) {
        this.collectionId = collectionId;
    }

    public String getCollectionName() {
        return collectionName;
    }

    public void setCollectionName(String collectionName) {
        this.collectionName = collectionName;
    }

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public boolean isVerified() {
        return verified;
    }

    public void setVerified(boolean verified) {
        this.verified = verified;
    }

    public boolean isEmailVisibility() {
        return emailVisibility;
    }

    public void setEmailVisibility(boolean emailVisibility) {
        this.emailVisibility = emailVisibility;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getCreated() {
        return created;
    }

    public void setCreated(String created) {
        this.created = created;
    }

    public String getUpdated() {
        return updated;
    }

    public void setUpdated(String updated) {
        this.updated = updated;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getTelephone() {
        return telephone;
    }

    public void setTelephone(String telephone) {
        this.telephone = telephone;
    }

    public String getAvatar() {
        return avatar;
    }

    public void setAvatar(String avatar) {
        this.avatar = avatar;
    }
}
