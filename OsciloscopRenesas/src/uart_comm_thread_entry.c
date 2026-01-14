#include <uart_comm_thread.h>

#define MARIME_MESAJ_UART 18U
#define MESAJE_NECESARE_UART 100U
uint8_t mesaj_uart[MARIME_MESAJ_UART];
extern uint16_t adc_buf[1000];

volatile uint32_t flagUart = 0U;
volatile uint8_t uart_send_index = 0U;
volatile uint16_t adc_buffer_offset = 0U;

uint8_t CRC_calculate(uint8_t length);

void user_uart_callback(uart_callback_args_t *p_args){
    (void)p_args;
    if(p_args->event == UART_EVENT_TX_COMPLETE)
    flagUart = 1U;
}

uint8_t CRC_calculate(uint8_t length){
    uint8_t crc=0, message[length];
    uint16_t divisor = 0x8D, dividend;
    int i, j;

    for(i=0; i<length-1; i++)
    {
        message[i] = mesaj_uart[i];
    }

    message[length-1]=0;

    dividend = (message[0] << 8) | message[1];
    for(j=15; j>=8; j--)
        if(dividend & (1 << j))
            dividend ^= divisor << (j-8);

    for(i=2; i<length; i++)
        {
            dividend = (dividend << 8) | message[i];

            for(j=15; j>=8; j--)
                if(dividend & (1 << j))
                    dividend ^= divisor << (j-8);
        }

    crc = (dividend % 256U);

    return crc;
}

/* New Thread entry function */
void uart_comm_thread_entry(void)
{
    while (1)
    {
        /*TODO asteptarea threadului de achizitie pentru date valide*/
        /*Trimitere pe UART*/
        for(uart_send_index = 0U; uart_send_index < MESAJE_NECESARE_UART; ){
            adc_buffer_offset = uart_send_index * 10U;
            mesaj_uart[0] = 10U;
            mesaj_uart[1] = uart_send_index;
            mesaj_uart[2] = (uint8_t)(adc_buf[0 + adc_buffer_offset] >> 4U);
            mesaj_uart[3] = (uint8_t)(((adc_buf[0 + adc_buffer_offset] & 0x0F) << 4U) + (adc_buf[1 + adc_buffer_offset] >> 8U));
            mesaj_uart[4] = (uint8_t)(adc_buf[1 + adc_buffer_offset] & 0xFF);
            mesaj_uart[5] = (uint8_t)(adc_buf[2 + adc_buffer_offset] >> 4U);
            mesaj_uart[6] = (uint8_t)(((adc_buf[2 + adc_buffer_offset] & 0x0F) << 4U) + (adc_buf[3 + adc_buffer_offset] >> 8U));
            mesaj_uart[7] = (uint8_t)(adc_buf[3 + adc_buffer_offset] & 0xFF);
            mesaj_uart[8] = (uint8_t)(adc_buf[4 + adc_buffer_offset] >> 4U);
            mesaj_uart[9] = (uint8_t)(((adc_buf[4 + adc_buffer_offset] & 0x0F) << 4U) + (adc_buf[5 + adc_buffer_offset] >> 8U));
            mesaj_uart[10] = (uint8_t)(adc_buf[5 + adc_buffer_offset] & 0xFF);
            mesaj_uart[11] = (uint8_t)(adc_buf[6 + adc_buffer_offset] >> 4U);
            mesaj_uart[12] = (uint8_t)(((adc_buf[6 + adc_buffer_offset] & 0x0F) << 4U) + (adc_buf[7 + adc_buffer_offset] >> 8U));
            mesaj_uart[13] = (uint8_t)(adc_buf[7 + adc_buffer_offset] & 0xFF);
            mesaj_uart[14] = (uint8_t)(adc_buf[8 + adc_buffer_offset] >> 4U);
            mesaj_uart[15] = (uint8_t)(((adc_buf[8 + adc_buffer_offset] & 0x0F) << 4U) + (adc_buf[9 + adc_buffer_offset] >> 8U));
            mesaj_uart[16] = (uint8_t)(adc_buf[9 + adc_buffer_offset] & 0xFF);
            mesaj_uart[17] = CRC_calculate(18);
            g_uart0.p_api->write(g_uart0.p_ctrl, mesaj_uart, MARIME_MESAJ_UART);
            while(flagUart == 0U);
            flagUart = 0;
            uart_send_index++;
        }
        tx_thread_sleep (0);
    }
}
