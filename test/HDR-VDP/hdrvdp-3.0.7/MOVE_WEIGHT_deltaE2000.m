function X = MOVE_WEIGHT_deltaE2000 ()

root = "test_move_weights";
test_number = 1;
dirs = [root+"/Test-"+test_number+"-SP/"+"0.33_0.34_0.33", ...
        root+"/Test-"+test_number+"-SP/"+"0.5_0.5_0", ...
        root+"/Test-"+test_number+"-SP/"+"0.5_0_0.5", ...
        root+"/Test-"+test_number+"-SP/"+"0_0.5_0.5", ...
        root+"/Test-"+test_number+"-SP/"+"0_0_1", ...
        root+"/Test-"+test_number+"-SP/"+"0_1_0", ...
        root+"/Test-"+test_number+"-SP/"+"1_0_0", ...
        root+"/Test-"+test_number+"-AOI"];
fullDir = root+"/FullMoving";

imgs = dir(fullDir + "/screen*.png");
imgs = {imgs(:).name};
[imgs, ~] = sort_nat(imgs);
perspNum = length(imgs);
fulls = cell(perspNum, 1);
for i = 1:perspNum
    full = double(imread(fullDir + "/" + imgs(i)));
    fulls{i} = full;
end

for i = 1:length(dirs)
    d1 = dirs(i);
    f = figure('visible','off');
    mbps = ["/4", "/10", "/20", "/40"];
    %mbps = "/full";
    for k = 1:length(mbps)
        X = zeros(perspNum, 1);
        d = d1 + mbps(k) + "/SCREEN";
        if exist(d + "/dE2000", 'dir')
            rmdir(d + "/dE2000", 's');
        end
        mkdir(d + "/dE2000");
        imgs = dir(d + "/screen*.png");
        imgs = {imgs(:).name};
        [imgs, ~] = sort_nat(imgs);
        for j = 1:length(imgs)
            file = d + "/" + imgs(j);
            disp(file);
            img = imread(file);
            diff = imcolordiff(img, fulls{j}, "Standard", "CIEDE2000",'kL',2,'K1',0.048,'K2',0.014);
            imagesc(diff);
            clim([0,100]);
            colorbar;
            saveas(f, d + "/dE2000/" + imgs(j));
            X(j, 1) = mean((mean(diff)));
        end
        plot(X)
        delete(d1 + mbps(k) + "/deltaE2000.png");
        saveas(f,  d1 + mbps(k) + "/deltaE2000.png");
        writematrix(X, d1 + mbps(k) + "/deltaE2000.xlsx");
    end
end





