package com.example.letsbook.Modal;
import com.example.letsbook.ModalDao.UserItem;
import com.google.gson.annotations.SerializedName;

import com.google.gson.annotations.SerializedName;

import java.io.Serializable;
import java.util.List;

public class User implements Serializable {
    private int page;
    private int perPage;

    private int totalItems;

    private int totalPages;

    private List<UserItem> items;

    public int getPage() {
        return page;
    }

    public int getPerPage() {
        return perPage;
    }

    public int getTotalItems() {
        return totalItems;
    }

    public int getTotalPages() {
        return totalPages;
    }

    public List<UserItem> getItems() {
        return items;
    }
}





