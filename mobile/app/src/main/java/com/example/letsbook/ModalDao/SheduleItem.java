package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class SheduleItem implements Serializable {
    private String id;
    private String collectionId;
    private String collectionName;
    private String created;
    private String updated;
    private String shedule;

    public SheduleItem(String id, String collectionId, String collectionName, String created, String updated, String shedule) {
        this.id = id;
        this.collectionId = collectionId;
        this.collectionName = collectionName;
        this.created = created;
        this.updated = updated;
        this.shedule = shedule;
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

    public String getShedule() {
        return shedule;
    }

    public void setShedule(String shedule) {
        this.shedule = shedule;
    }
}
