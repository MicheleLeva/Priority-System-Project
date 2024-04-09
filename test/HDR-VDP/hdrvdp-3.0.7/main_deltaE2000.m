function X = main_deltaE2000 ()

root = "test";
dirs = [root+"/Test-4-SP", root+"/Test-4-AOI"];
full = imread('test\full.png');

for i = 1:length(dirs)
    d1 = dirs(i);
    f = figure('visible','off');
    mbps = ["/4", "/10", "/20", "/40"];
    %mbps = "/full";
    for k = 1:length(mbps)
        X = zeros(46, 2);
        d = d1 + mbps(k) + "/SCREEN";
        mkdir(d + "/dE2000");
        imgs = dir(d + "/screen*.png");
        imgs = {imgs(:).name};
        [imgs, ~] = sort_nat(imgs);
        for j = 1:length(imgs)
            file = d + "/" + imgs(j);
            disp(file);
            img = imread(file);
            diff = imcolordiff(img, full, "Standard", "CIEDE2000",'kL',2,'K1',0.048,'K2',0.014);
            imagesc(diff);
            colormap gray;
            clim([0,100]);
            colorbar;
            saveas(f, d + "/dE2000/" + imgs(j));
            X(j, 1) = str2double(erase(erase(imgs(j), "screen_"), ".png"));
            X(j, 2) = mean((mean(diff)));
        end
        plot(X)
        saveas(f,  d1 + mbps(k) + "/deltaE2000.png");
        writematrix(X.', d1 + mbps(k) + "/deltaE2000.csv");
    end
end





