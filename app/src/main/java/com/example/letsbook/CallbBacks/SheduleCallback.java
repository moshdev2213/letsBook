package com.example.letsbook.CallbBacks;

import com.example.letsbook.Modal.User;
import com.example.letsbook.ModalDao.SheduleItem;
import com.example.letsbook.ModalDao.SheduleRes;

public interface SheduleCallback {
    void onLoadGetSheduleData(SheduleItem sheduleItem);

}
